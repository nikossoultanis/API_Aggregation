using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;

namespace ApiAggregationService.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NewsService(HttpClient httpClient, IOptions<NewsConfig> config)
        {
            _httpClient = httpClient;
            _apiKey = config.Value.ApiKey;
        }

        public async Task<string> GetNewsAsync(string location)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://gnews.io/api/v4/top-headlines?category={location}&lang=el&&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch(HttpRequestException)
            {
                return "News data currently unavailable.";
            }
        }
    }
}
