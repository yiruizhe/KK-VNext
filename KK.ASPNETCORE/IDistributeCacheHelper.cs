
using Microsoft.Extensions.Caching.Distributed;

namespace KK.ASPNETCORE
{
    public interface IDistributeCacheHelper
    {
        TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions,TResult?> valueFactory, int baseExpireTime = 60);
        Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions,Task<TResult?>> valueFactory, int baseExpireTime = 60);
        void Remove(string cacheKey);
        Task RemoveAsync(string cacheKey);
    }
}
