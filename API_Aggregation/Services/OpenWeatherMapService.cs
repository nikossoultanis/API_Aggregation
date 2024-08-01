using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
                string response = "";
                var stopwatch = Stopwatch.StartNew();

                ResilientHttpClient resilientHttpClient = new ResilientHttpClient();
                response = await resilientHttpClient.GetDataWithFallbackAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}");        
                
                stopwatch.Stop();

                _statisticsService.RecordRequest("OpenWeatherMap", stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (HttpRequestException)
            {
                return "Weather data currently unavailable.";
            }
        }
    }
}
