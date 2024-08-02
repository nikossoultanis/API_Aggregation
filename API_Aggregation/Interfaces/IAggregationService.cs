namespace API_Aggregation.Interfaces
{
    /// <summary>
    /// Provides methods to retrieve aggregated data from various services.
    /// </summary
    public interface IAggregationService
    {
        /// <summary>
         /// Asynchronously retrieves aggregated data for the specified location and query with optional date-time filtering.
         /// </summary>
         /// <param name="location">The location used to search for data.</param>
         /// <param name="query">The query string used for searching data.</param>
         /// <param name="dateTimeFiltering">Indicates whether date-time filtering should be applied.</param>
         /// <param name="fromDate">The starting date for filtering the data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
         /// <param name="toDate">The ending date for filtering the data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
         /// <returns>
         /// A task that represents the asynchronous operation.
         /// The task result contains an <see cref="AggregatedData"/> object with the aggregated data.
         /// </returns>
        Task<AggregatedData> GetAggregatedDataAsync(string location, string query,bool dateTimeFiltering, string fromDate, string toDate);
    }
    /// <summary>
    /// Represents the aggregated data retrieved from various services.
    /// </summary
    public class AggregatedData
    {
        /// <summary>
        /// Gets or sets the weather data.
        /// </summary>
        public string WeatherData { get; set; }

        /// <summary>
        /// Gets or sets the Twitter data.
        /// </summary>
        public string TwitterData { get; set; }

        /// <summary>
        /// Gets or sets the news data.
        /// </summary>
        public string NewsData { get; set; }

        /// <summary>
        /// Gets or sets the music data.
        /// </summary>
        public string MusicData { get; set; }

        /// <summary>
        /// Gets or sets the country data.
        /// </summary>
        public string CountryData { get; set; }
    }
}
