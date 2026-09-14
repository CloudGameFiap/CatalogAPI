using CloudGameCatalog.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CloudGameCatalog.Infrastructure.Cache
{
    public class RedisCacheService: ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var valor = await _distributedCache.GetStringAsync(key);
            return valor is null ? default : JsonSerializer.Deserialize<T>(valor);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        public Task RemoveAsync(string key) => _distributedCache.RemoveAsync(key);
    }
}
