using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace API_Aggregation.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken;

        public TwitterService(HttpClient httpClient, IOptions<TwitterConfig> config)
        {
            _httpClient = httpClient;
            _bearerToken = config.Value.BearerToken;
        }

        public async Task<string> GetTweetsAsync(string query)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitter.com/2/tweets/search/recent?query={Uri.EscapeDataString("Greece news")}&max_results=50");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch(HttpRequestException ex)
            {
                return "Tweeter "+ ex.Message;
            }
        }
    }
}
