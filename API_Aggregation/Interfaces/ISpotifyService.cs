﻿namespace API_Aggregation.Interfaces
{
    public interface ISpotifyService
    {
        Task<string> GetMusicDataAsync(string location);
    }
}
