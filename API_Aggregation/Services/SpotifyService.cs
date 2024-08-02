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
        // API keys
        private readonly string _clientId;
        private readonly string _clientIdSecret;
        // cache object for caching previous api calls
        private APICaching _cache;
        // statistic object to give statistics to the user from different endpoint
        private RequestStatisticsService _sp;
        public SpotifyService(HttpClient httpClient, string clientID, string clientSecretID, APICaching cache, RequestStatisticsService sp/*IOptions<SpotifyConfig> config*/)
        {
            _clientId = clientID;
            _clientIdSecret = clientSecretID;
            _cache = cache;
            _sp = sp;
        }
        /// <summary>
        /// Asynchronously retrieves music data from Spotify based on the specified location and optional date-time filtering.
        /// </summary>
        /// <param name="location">The location used to search for music data.</param>
        /// <param name="dateTimeFiltering">Indicates whether date-time filtering should be applied.</param>
        /// <param name="fromDate">The starting date for filtering the music data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the music data as a string.
        /// </returns>
        public async Task<string> GetMusicDataAsync(string location, bool dateTimeFiltering, string fromDate)
        {

            // cache value is the Key that we are looking for into the cache memory.
            string cacheValue = "";
            if (dateTimeFiltering)
                cacheValue = $"{location}-{fromDate}";
            else
                cacheValue = $"{location}";
            string ApiResponse = "";
            var stopwatch = Stopwatch.StartNew();

            // GetOrAddAsync function, will first check if cache has the corresponding Key, otherwise it will continue and call the API
            string mostRecentResponse = await _cache.GetOrAddAsync(cacheValue, async () =>
            { 
                try
                {
                    // We need to give to http header, the authenication. Spotify API is very strict.
                    string tokenUrl = "https://accounts.spotify.com/api/token";
                    using HttpClient client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientIdSecret}")));
                    request.Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                    // when auth is ready, we continue by sending the Request.
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(responseBody);
                        var accessToken = json["access_token"]?.ToString();

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        HttpResponseMessage new_response = new HttpResponseMessage();

                        ResilientHttpClient resilientHttpClient = new ResilientHttpClient(client);
                        // Request was successful, we got the data and we parse them to string and eventually return them to the user
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
                // catch case if the request fails. Even if it is forbidden or connection failed etc.
                catch (Exception ex)
                {
                    return "Spotify API call error: " + ex.Message;
                }
            });

            stopwatch.Stop();
            // save the stats to the Statistics Service
            _sp.RecordRequest("Spotify", stopwatch.ElapsedMilliseconds);

            return mostRecentResponse;

        }
    }
}
