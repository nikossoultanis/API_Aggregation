using System;
using System.Threading.Tasks;
using API_Aggregation.Interfaces;
using API_Aggregation.Services;
using Moq;
using Xunit;

namespace API_Aggregation.Tests
{
    public class AggregationServiceTests
    {
        private readonly Mock<IOpenWeatherMapService> _weatherServiceMock;
        private readonly Mock<ITwitterService> _twitterServiceMock;
        private readonly Mock<INewsService> _newsServiceMock;
        private readonly Mock<ISpotifyService> _spotifyServiceMock;
        private readonly Mock<ICountryService> _countryServiceMock;
        private readonly AggregationService _aggregationService;

        public AggregationServiceTests()
        {
            _weatherServiceMock = new Mock<IOpenWeatherMapService>();
            _twitterServiceMock = new Mock<ITwitterService>();
            _newsServiceMock = new Mock<INewsService>();
            _spotifyServiceMock = new Mock<ISpotifyService>();
            _countryServiceMock = new Mock<ICountryService>();

            _aggregationService = new AggregationService(
                _weatherServiceMock.Object,
                _twitterServiceMock.Object,
                _newsServiceMock.Object,
                _spotifyServiceMock.Object,
                _countryServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAggregatedDataAsync_ReturnsAggregatedData()
        {
            // Arrange
            _weatherServiceMock.Setup(s => s.GetWeatherDataAsync(It.IsAny<string>())).ReturnsAsync("{\"weather\":\"sunny\"}");
            _twitterServiceMock.Setup(s => s.GetTweetsAsync(It.IsAny<string>())).ReturnsAsync("[{\"text\":\"tweet1\"}, {\"text\":\"tweet2\"}]");
            _newsServiceMock.Setup(s => s.GetNewsAsync(It.IsAny<string>(), true, "2022-01-01T12:30:45Z", "2024-01-01T12:30:45Z")).ReturnsAsync("[{\"title\":\"news1\"}, {\"title\":\"news2\"}]");
            _spotifyServiceMock.Setup(s => s.GetMusicDataAsync(It.IsAny<string>(), true, "2022-01-01T12:30:45Z")).ReturnsAsync("[{\"track\":\"song1\"}, {\"track\":\"song2\"}]");
            _countryServiceMock.Setup(s => s.GetCountryDataAsync(It.IsAny<string>())).ReturnsAsync("[{\"repo\":\"country1\"}, {\"repo\":\"country2\"}]");

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync("Greece", "Greece", true, "2022-01-01T12:30:45Z", "2024-01-01T12:30:45Z");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("{\"weather\":\"sunny\"}", result.WeatherData);
            Assert.Equal("[{\"text\":\"tweet1\"}, {\"text\":\"tweet2\"}]", result.TwitterData);
            Assert.Equal("[{\"title\":\"news1\"}, {\"title\":\"news2\"}]", result.NewsData);
            Assert.Equal("[{\"track\":\"song1\"}, {\"track\":\"song2\"}]", result.MusicData);
            Assert.Equal("[{\"repo\":\"country1\"}, {\"repo\":\"country2\"}]", result.CountryData);
        }
    }
}
