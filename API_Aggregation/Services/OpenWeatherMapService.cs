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
        private readonly APICaching _cache;

        public OpenWeatherMapService(HttpClient httpClient, IOptions<OpenWeatherMapConfig> config,APICaching cache ,RequestStatisticsService statisticsService)
        {
            _httpClient = httpClient;
            // API Key Entry
            _apiKey = config.Value.ApiKey;
            _statisticsService = statisticsService;
            _cache = cache;
        }

        // Task for async API GET

        /// <summary>
        /// Asynchronously retrieves weather data for the specified location from OpenWeatherMap.
        /// </summary>
        /// <param name="location">The location used to search for weather data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the weather data as a string.
        /// If the weather data is currently unavailable, returns a message indicating this.
        /// </returns>
        public async Task<string> GetWeatherDataAsync(string location)
        {
            try
            {
                string response = "";
                // cache value is the key we are looking for into the cache memory
                string cacheValue = $"{location}";
                var stopwatch = Stopwatch.StartNew();
                // if not found, then we continue with the GET request
                string mostRecentResponse = await _cache.GetOrAddAsync(cacheValue, async () =>
                {
                    ResilientHttpClient resilientHttpClient = new ResilientHttpClient();
                    response = await resilientHttpClient.GetDataWithFallbackAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_apiKey}");
                    return response;
                });
                stopwatch.Stop();

                // statistics Service
                _statisticsService.RecordRequest("OpenWeatherMap", stopwatch.ElapsedMilliseconds);
                return mostRecentResponse;
            }
            // Faultproof
            catch (HttpRequestException)
            {
                return "Weather data currently unavailable.";
            }
        }
    }
}
