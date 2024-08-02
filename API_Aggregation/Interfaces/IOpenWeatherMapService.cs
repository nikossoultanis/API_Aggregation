namespace API_Aggregation.Interfaces
{
    /// <summary>
    /// Provides methods to retrieve weather data from OpenWeatherMap.
    /// </summary>
    public interface IOpenWeatherMapService
    {
        /// <summary>
        /// Asynchronously retrieves weather data for the specified location.
        /// </summary>
        /// <param name="location">The location used to search for weather data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the weather data as a string.
        /// </returns>
        Task<string> GetWeatherDataAsync(string location);
    }
}
