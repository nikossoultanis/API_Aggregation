namespace API_Aggregation.Interfaces
{
    public interface IOpenWeatherMapService
    {
        Task<string> GetWeatherDataAsync(string location);
    }
}
