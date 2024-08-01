using System.Net.Http;
using System.Threading.Tasks;
using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ApiAggregationService.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public GitHubService(HttpClient httpClient, IOptions<GitHubConfig> config)
        {
            _httpClient = httpClient;
            _accessToken = config.Value.AccessToken;
        }

        public async Task<string> GetRepositoryDataAsync(string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/search/repositories?q={query}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            request.Headers.UserAgent.ParseAdd("ApiAggregationService");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}