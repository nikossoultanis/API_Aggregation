using System.Net.Http;
using System.Threading.Tasks;
using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace API_Aggregation.Services
{
    // Inheritance of the main Interface
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapService(HttpClient httpClient, IOptions<OpenWeatherMapConfig> config)
        {
            _httpClient = httpClient;
            // API Key Entry
            _apiKey = config.Value.ApiKey;
        }

        // Task for async API GET
        public async Task<string> GetWeatherDataAsync(string location)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return "Weather data currently unavailable.";
            }
        }
    }
}
