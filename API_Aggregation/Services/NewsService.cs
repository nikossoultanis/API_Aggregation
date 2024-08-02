using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace API_Aggregation.Services
{
    public class NewsService : INewsService
    {
        private readonly string _apiKey;
        private readonly RequestStatisticsService _statisticsService;
        private APICaching _cache;

        public NewsService(IOptions<NewsConfig> config, APICaching cache,RequestStatisticsService statisticsService)
        {
            _apiKey = config.Value.ApiKey;
            _statisticsService = statisticsService;
            _cache = cache;
        }
        /// <summary>
        /// Asynchronously retrieves news data based on the specified location and optional date-time filtering.
        /// </summary>
        /// <param name="location">The location used to search for news data.</param>
        /// <param name="dateTimeFiltering">Indicates whether date-time filtering should be applied.</param>
        /// <param name="fromDate">The starting date for filtering the news data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
        /// <param name="toDate">The ending date for filtering the news data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the news data as a string.
        /// If the news data is currently unavailable, returns a message indicating this.
        /// </returns>
        public async Task<string> GetNewsAsync(string location, bool dateTimeFiltering, string fromDate, string toDate)
        {
            try
            {
                string response = "";
                var sports = "sports";
                // cache value is the key we are looking for into the cache memory
                // it is different, depending the date filtering variable.
                // if we want to filter by date, then we need to search for a api call that included datetime
                string cacheValue = "";
                if (dateTimeFiltering)
                    cacheValue = $"{location}-{fromDate}-{toDate}";
                // Timer for Statistics
                var stopwatch = Stopwatch.StartNew();
                string mostRecentResponse = await _cache.GetOrAddAsync(cacheValue, async () =>
                {
                    ResilientHttpClient resilientHttpClient = new ResilientHttpClient();

                    if (dateTimeFiltering)
                        response = await resilientHttpClient.GetDataWithFallbackAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&from={fromDate}&to={toDate}&lang=el&apikey={_apiKey}");
                    else
                        response = await resilientHttpClient.GetDataWithFallbackAsync($"https://gnews.io/api/v4/search?q={sports}&country={location}&lang=el&&apikey={_apiKey}");
                    return response;
                });
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
                return mostRecentResponse;
            }
            catch(HttpRequestException)
            {
                return "News data currently unavailable.";
            }
        }
    }
}
