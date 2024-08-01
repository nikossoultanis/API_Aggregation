using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
namespace API_Aggregation
{
    public class ResilientHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy;

        // the input is for the spotify auth, because with import the auth inside the http client, so we must use it as input here
        public ResilientHttpClient(HttpClient? inputClient = null)
        {
            if(inputClient!=null)
                httpClient = inputClient;
            else
                httpClient = new HttpClient();

            // Configure retry policy with exponential backoff
            retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds} seconds due to {result.Result.StatusCode}");
                    });

            // Configure circuit breaker policy
            circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60),
                    onBreak: (result, timeSpan) =>
                    {
                        Console.WriteLine($"Circuit broken due to {result.Result.StatusCode}. Duration: {timeSpan.Seconds} seconds");
                    },
                    onReset: () => Console.WriteLine("Circuit reset."),
                    onHalfOpen: () => Console.WriteLine("Circuit in half-open state. Next call is a trial.")
                );
        }

        public async Task<string> GetDataWithFallbackAsync(string apiUrl)
        {
            try
            {
                var response = await retryPolicy.ExecuteAsync(() => circuitBreakerPolicy.ExecuteAsync(() => httpClient.GetAsync(apiUrl)));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                // Fallback to cached data or default behavior
                return "{\"message\": \"Fallback data\"}";
            }
        }
    }
}