namespace API_Aggregation.Interfaces
{
    public interface ITwitterService
    {
        Task<string> GetTweetsAsync(string query);
    }
}
