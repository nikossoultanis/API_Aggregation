using System.Diagnostics;
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
        private readonly RequestStatisticsService _statisticsService;


        public OpenWeatherMapService(HttpClient httpClient, IOptions<OpenWeatherMapConfig> config, RequestStatisticsService statisticsService)
        {
            _httpClient = httpClient;
            // API Key Entry
            _apiKey = config.Value.ApiKey;
            _statisticsService = statisticsService;

        }

        // Task for async API GET
        public async Task<string> GetWeatherDataAsync(string location)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                stopwatch.Stop();
                _statisticsService.RecordRequest("OpenWeatherMap", stopwatch.ElapsedMilliseconds);
                return data;
            }
            catch (HttpRequestException)
            {
                return "Weather data currently unavailable.";
            }
        }
    }
}
