namespace API_Aggregation.Interfaces
{
    public interface IAggregationService
    {
        Task<AggregatedData> GetAggregatedDataAsync(string location, string query,bool dateTimeFiltering, string fromDate, string toDate);
    }

    public class AggregatedData
    {
        public string WeatherData { get; set; }
        public string TwitterData { get; set; }
        public string NewsData { get; set; }
        public string MusicData { get; set; }
        public string CountryData { get; set; }
    }
}
