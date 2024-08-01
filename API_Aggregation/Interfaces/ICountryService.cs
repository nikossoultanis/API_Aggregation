namespace API_Aggregation.Interfaces
{
    public interface ICountryService
    {
        Task<string> GetCountryDataAsync(string query);
    }
}
