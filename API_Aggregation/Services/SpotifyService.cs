using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;

namespace API_Aggregation.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly string _clientId;
        private readonly string _clientIdSecret;
        private APICaching _cache;
        private RequestStatisticsService _sp;
        public SpotifyService(HttpClient httpClient, string clientID, string clientSecretID, APICaching cache, RequestStatisticsService sp/*IOptions<SpotifyConfig> config*/)
        {
            _clientId = clientID;
            _clientIdSecret = clientSecretID;
            _cache = cache;
            _sp = sp;
        }

        public async Task<string> GetMusicDataAsync(string location, bool dateTimeFiltering, string fromDate)
        {

            string cacheValue = "";
            if (dateTimeFiltering)
                cacheValue = $"{location}-{fromDate}";
            else
                cacheValue = $"{location}";
            string ApiResponse = "";
            var stopwatch = Stopwatch.StartNew();

            string mostRecentResponse = await _cache.GetOrAddAsync(cacheValue, async () =>
            { 
                try
                {
                    string tokenUrl = "https://accounts.spotify.com/api/token";
                    using HttpClient client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientIdSecret}")));
                    request.Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(responseBody);
                        var accessToken = json["access_token"]?.ToString();

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        HttpResponseMessage new_response = new HttpResponseMessage();

                        ResilientHttpClient resilientHttpClient = new ResilientHttpClient(client);

                        if (dateTimeFiltering)
                            ApiResponse = await resilientHttpClient.GetDataWithFallbackAsync($"https://api.spotify.com/v1/search?q={fromDate.Substring(0, 5)}&type=track&country={location}&market={Uri.EscapeDataString("GR")}&&limit=10");
                        else
                            ApiResponse = await resilientHttpClient.GetDataWithFallbackAsync($"https://api.spotify.com/v1/search?q={Uri.EscapeDataString("track")}&type=track&country={location}&market={Uri.EscapeDataString("GR")}&limit=10");

                        return ApiResponse;
                    }
                    else
                    {
                        Console.WriteLine("Error getting access token: " + response.ReasonPhrase);
                        return "Error getting access token: " + response.ReasonPhrase;
                        }
                    }
                    catch (Exception ex)
                    {
                        return "Spotify API call error: " + ex.Message;
                    }
            });

            stopwatch.Stop();
            _sp.RecordRequest("Spotify", stopwatch.ElapsedMilliseconds);

            return mostRecentResponse;

        }
    }
}
