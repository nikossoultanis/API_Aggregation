using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using Polly.Retry;
using System.Diagnostics;

namespace API_Aggregation.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly RequestStatisticsService _statisticsService;

        public NewsService(HttpClient httpClient, IOptions<NewsConfig> config, RequestStatisticsService statisticsService)
        {
            _httpClient = httpClient;
            _apiKey = config.Value.ApiKey;
            _statisticsService = statisticsService;

        }

        public async Task<string> GetNewsAsync(string location, bool dateTimeFiltering, string fromDate, string toDate)
        {
            try
            {
                string response = "";
                var sports = "sports";

                // Timer for Statistics
                var stopwatch = Stopwatch.StartNew();

                ResilientHttpClient resilientHttpClient = new ResilientHttpClient();

                if (dateTimeFiltering)
                    response = await resilientHttpClient.GetDataWithFallbackAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&from={fromDate}&to={toDate}&lang=el&apikey={_apiKey}");
                else
                    response = await resilientHttpClient.GetDataWithFallbackAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&lang=el&&apikey={_apiKey}");

                /*
                 * code where the api call is done without fallback mechanism
       
                if (dateTimeFiltering)
                    response = await _httpClient.GetAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&from={fromDate}&to={toDate}&lang=el&apikey={_apiKey}");
                else
                    response = await _httpClient.GetAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&lang=el&&apikey={_apiKey}");
                
                response.EnsureSuccessStatusCode();

                // if response if OK we continue by reading the response
                // otherwise we go the the catch and return a message

                var data = await response.Content.ReadAsStringAsync();
                */

                stopwatch.Stop();
                // statistic service is responsible for saving the info data about an API call.
                // in this example the info is the time required for the api call.
                // in swagger we can see all the saved data for every API call we did.
                _statisticsService.RecordRequest("GNewsAPI", stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch(HttpRequestException)
            {
                return "News data currently unavailable.";
            }
        }
    }
}
