using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace API_Aggregation.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly RequestStatisticsService _statisticsService;

        public CountryService(HttpClient httpClient, RequestStatisticsService statisticsService)
        {   
            _httpClient = httpClient;
            _statisticsService = statisticsService;
        }

        public async Task<string> GetCountryDataAsync(string query)
        {
            try
            {
                // Timer for Statistics
                var stopwatch = Stopwatch.StartNew();

                // HTTP request to the API endpoint
                var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{query}");
                response.EnsureSuccessStatusCode();
                // if response if OK we continue by reading the response
                // otherwise we go the the catch and return a message
                var data = await response.Content.ReadAsStringAsync();

                stopwatch.Stop();
                // statistic service is responsible for saving the info data about an API call.
                // in this example the info is the time required for the api call.
                // in swagger we can see all the saved data for every API call we did.
                _statisticsService.RecordRequest("CountryAPI", stopwatch.ElapsedMilliseconds);
                return data;
            }
            catch (HttpRequestException)
            {
                return "Country data currently unavailable.";
            }
        }
    }
}