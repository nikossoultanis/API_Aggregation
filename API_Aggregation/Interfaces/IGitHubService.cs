namespace API_Aggregation.Interfaces
{
    public interface IGitHubService
    {
        Task<string> GetRepositoryDataAsync(string query);
    }
}
