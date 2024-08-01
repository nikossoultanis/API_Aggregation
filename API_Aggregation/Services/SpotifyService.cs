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

        public async Task<string> GetMusicDataAsync(string location, bool dateTimeFiltering, string fromDate)
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
                    if (dateTimeFiltering)
                        // substring gets only Year from the datetime.
                        new_response = await client.GetAsync($"https://api.spotify.com/v1/search?q={fromDate.Substring(0, 5)}&type=track&country={location}&market={Uri.EscapeDataString("GR")}&&limit=10");
                    else
                        new_response = await client.GetAsync($"https://api.spotify.com/v1/search?q={Uri.EscapeDataString("track")}&type=track&country={location}&market={Uri.EscapeDataString("GR")}&limit=10");

                    if (response.IsSuccessStatusCode)
                    {
                        string new_responseBody = await new_response.Content.ReadAsStringAsync();
                        return new_responseBody;
                    }
                    else
                    {
                        Console.WriteLine("Error getting top tracks: " + response.ReasonPhrase);
                        return "Error getting tracks: " + response.ReasonPhrase;
                    }
                }
                else
                {
                    Console.WriteLine("Error getting access token: " + response.ReasonPhrase);
                    return "Error getting access token: " + response.ReasonPhrase;
                }
            }catch(Exception ex) 
            {
                return "Spotify API call error: " +ex.Message;
            }
        }
    }
}
