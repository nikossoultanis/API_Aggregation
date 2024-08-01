using API_Aggregation.Interfaces;
using API_Aggregation.Services;
using Moq;
using Xunit;

namespace ApiAggregationService.Tests
{
    public class AggregationServiceTests
    {
        [Fact]
        public async Task GetAggregatedDataAsync_ReturnsAggregatedData()
        {
            // Arrange
            var weatherServiceMock = new Mock<IOpenWeatherMapService>();
            var twitterServiceMock = new Mock<ITwitterService>();
            var newsServiceMock = new Mock<INewsService>();
            var spotifyServiceMock = new Mock<ISpotifyService>();
            var gitHubServiceMock = new Mock<IGitHubService>();

            weatherServiceMock.Setup(s => s.GetWeatherDataAsync(It.IsAny<string>())).ReturnsAsync("WeatherData");
            twitterServiceMock.Setup(s => s.GetTweetsAsync(It.IsAny<string>())).ReturnsAsync("TwitterData");
            newsServiceMock.Setup(s => s.GetNewsAsync(It.IsAny<string>())).ReturnsAsync("NewsData");
            spotifyServiceMock.Setup(s => s.GetMusicDataAsync(It.IsAny<string>())).ReturnsAsync("MusicData");
            gitHubServiceMock.Setup(s => s.GetRepositoryDataAsync(It.IsAny<string>())).ReturnsAsync("GitHubData");

            var aggregationService = new AggregationService(
                weatherServiceMock.Object,
                twitterServiceMock.Object,
                newsServiceMock.Object,
                spotifyServiceMock.Object,
                gitHubServiceMock.Object
            );

            // Act
            var result = await aggregationService.GetAggregatedDataAsync("location", "query");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WeatherData", result.WeatherData);
            Assert.Equal("TwitterData", result.TwitterData);
            Assert.Equal("NewsData", result.NewsData);
            Assert.Equal("MusicData", result.MusicData);
            Assert.Equal("GitHubData", result.GitHubData);
        }
    }
}
