using API_Aggregation.Configurations;
using API_Aggregation.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_Aggregation.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _clientId;
        private readonly string _clientIdSecret;

        public SpotifyService(HttpClient httpClient, IOptions<SpotifyConfig> config)
        {
            _httpClient = httpClient;
            _accessToken = config.Value.AccessToken;
            _clientId = config.Value.ClientID;
            _clientIdSecret = config.Value.ClientIDSecret;
        }

        public async Task<string> GetMusicDataAsync(string location)
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

                HttpResponseMessage new_response = await client.GetAsync($"https://api.spotify.com/v1/browse/new-releases?country={location}&limit=10");
                if (response.IsSuccessStatusCode)
                {
                    string new_responseBody = await new_response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(new_responseBody);
                    string topSong = "";
                    foreach (var item in result["albums"]["items"])
                    {
                        Console.WriteLine($"Album: {item["name"]}, Artist: {item["artists"][0]["name"]}");
                        topSong+= $" Album: {item["name"]}, Artist: {item["artists"][0]["name"]}";
                    }
                    return topSong;
                }
                else
                {
                    Console.WriteLine("Error getting top tracks: " + response.ReasonPhrase);
                    return "Error getting top tracks: " + response.ReasonPhrase;
                }
            }
            else
            {
                Console.WriteLine("Error getting access token: " + response.ReasonPhrase);
                return "Error getting access token: " + response.ReasonPhrase;
            }
        }
    }
}
