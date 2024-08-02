namespace API_Aggregation.Interfaces
{   
     /// <summary>
     /// Provides methods to retrieve music data from Spotify.
     /// </summary>
    public interface ISpotifyService
    {        
        /// <summary>
        /// Asynchronously retrieves music data based on the specified location and optional filters.
        /// </summary>
        /// <param name="location">The location used to search for music data.</param>
        /// <param name="dateTimeFiltering">Indicates whether date-time filtering should be applied.</param>
        /// <param name="fromDate">The starting date for filtering the music data. This parameter is used only if <paramref name="dateTimeFiltering"/> is true.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the music data as a string.
        /// </returns>
        Task<string> GetMusicDataAsync(string location, bool dateTimeFiltering, string fromDate);
    }
}
