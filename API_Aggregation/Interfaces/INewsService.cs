namespace API_Aggregation.Interfaces
{
    public interface INewsService
    {
        Task<string> GetNewsAsync(string query, bool dateTimeFiltering, string fromDate, string toDate);
    }
}