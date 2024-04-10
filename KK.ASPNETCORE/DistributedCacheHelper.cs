using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace KK.ASPNETCORE;

public class DistributedCacheHelper : IDistributeCacheHelper
{
    private readonly IDistributedCache cache;

    public DistributedCacheHelper(IDistributedCache cache)
    {
        this.cache = cache;
    }

    private DistributedCacheEntryOptions CreateOptions(int baseExpireSeconds)
    {
        var cacheOptions = new DistributedCacheEntryOptions();
        int expireSeconds = Random.Shared.Next(baseExpireSeconds * 2);
        cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds);
        cacheOptions.SlidingExpiration = TimeSpan.FromSeconds(baseExpireSeconds / 2);
        return cacheOptions;
    }

    public TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int baseExpireTime = 60)
    {
        string json = cache.GetString(cacheKey);
        if (string.IsNullOrEmpty(json))
        {
            var options = CreateOptions(baseExpireTime);
            var res = valueFactory(options);
            cache.SetString(cacheKey, JsonSerializer.Serialize<TResult>(res), options);
            return res;
        }
        cache.Refresh(cacheKey);
        return JsonSerializer.Deserialize<TResult>(json);
    }

    public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int baseExpireTime = 60)
    {
        string json = await cache.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(json))
        {
            var options = CreateOptions(baseExpireTime);
            var res = await valueFactory(options);
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize<TResult>(res), options);
            return res;
        }
        await cache.RefreshAsync(cacheKey);
        return JsonSerializer.Deserialize<TResult>(json);
    }

    public void Remove(string cacheKey)
    {
        cache.Remove(cacheKey);
    }

    public Task RemoveAsync(string cacheKey)
    {
        return cache.RemoveAsync(cacheKey);
    }
}
