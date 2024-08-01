using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace API_Aggregation.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
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
                var stopwatch = Stopwatch.StartNew();

                var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{query}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                stopwatch.Stop();
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