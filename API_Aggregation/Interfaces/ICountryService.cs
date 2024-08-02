namespace API_Aggregation.Interfaces
{    
     /// <summary>
     /// Provides methods to retrieve country data.
     /// </summary>
    public interface ICountryService
    {   
        /// <summary>
        /// Asynchronously retrieves country data based on the specified query.
        /// </summary>
        /// <param name="query">The query string used to search for country data.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the country data as a string.
        /// </returns>
        Task<string> GetCountryDataAsync(string query);
    }
}
