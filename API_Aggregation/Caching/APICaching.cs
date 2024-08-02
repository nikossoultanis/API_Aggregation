using Microsoft.Extensions.Caching.Memory;

namespace API_Aggregation
{
    public class APICaching
    {
        private readonly IMemoryCache _cache;

        public APICaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> fetchFunction, TimeSpan? slidingExpiration = null)
        {
            if (!_cache.TryGetValue(cacheKey, out T cacheEntry))
            {
                cacheEntry = await fetchFunction();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? TimeSpan.FromMinutes(30)
                };

                _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}
