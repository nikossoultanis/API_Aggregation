using API_Aggregation.Interfaces;
using System.Diagnostics;

namespace API_Aggregation.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly RequestStatisticsService _statisticsService;
        private APICaching _cache;

        public CountryService(HttpClient httpClient, APICaching cache, RequestStatisticsService sp)
        {   
            _httpClient = httpClient;
            _statisticsService = sp;
            _cache = cache;
        }
        /// <summary>
        /// Asynchronously retrieves country data based on the specified query.
        /// </summary>
        /// <param name="query">The query string used to search for country data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the country data as a string.
        /// If the country data is currently unavailable, returns a message indicating this.
        /// </returns>
        public async Task<string> GetCountryDataAsync(string query)
        {
            try
            {                
                // cache value is the key we are looking for into the cache memory
                string cacheValue = $"{query}";
                string response="";
                // Timer for Statistics
                var stopwatch = Stopwatch.StartNew();
                string mostRecentResponse = await _cache.GetOrAddAsync(cacheValue, async () =>
                {
                    // HTTP request to the API endpoint
                    ResilientHttpClient resilientHttpClient = new ResilientHttpClient();

                    response = await resilientHttpClient.GetDataWithFallbackAsync($"https://restcountries.com/v3.1/name/{query}");

                    return response;
                });
                
                stopwatch.Stop();

                // statistic service is responsible for saving the info data about an API call.
                // in this example the info is the time required for the api call.
                // in swagger we can see all the saved data for every API call we did.
                _statisticsService.RecordRequest("CountryAPI", stopwatch.ElapsedMilliseconds);
                return mostRecentResponse;
            }
            catch (HttpRequestException)
            {
                return "Country data currently unavailable.";
            }
        }
    }
}