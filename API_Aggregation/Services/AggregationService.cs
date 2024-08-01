using API_Aggregation.Interfaces;

namespace API_Aggregation.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IOpenWeatherMapService _weatherService;
        private readonly ITwitterService _twitterService;
        private readonly INewsService _newsService;
        private readonly ISpotifyService _spotifyService;
        private readonly ICountryService _countryService;

        public AggregationService(IOpenWeatherMapService weatherService, ITwitterService twitterService, INewsService newsService, ISpotifyService spotifyService, ICountryService countryService)
        {
            _weatherService = weatherService;
            _twitterService = twitterService;
            _newsService = newsService;
            _spotifyService = spotifyService;
            _countryService = countryService;
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(string location, string query)
        {
            var weatherTask = _weatherService.GetWeatherDataAsync(location);
            var twitterTask = _twitterService.GetTweetsAsync(query);
            var newsTask = _newsService.GetNewsAsync(location);
            var musicTask = _spotifyService.GetMusicDataAsync(location);
            var countryTask = _countryService.GetCountryDataAsync(query);

            await Task.WhenAll(weatherTask, twitterTask, newsTask, musicTask, countryTask);

            return new AggregatedData
            {
                WeatherData = await weatherTask,
                TwitterData = await twitterTask,
                NewsData = await newsTask,
                MusicData = await musicTask,
                CountryData = await countryTask
            };
        }
    }
}
