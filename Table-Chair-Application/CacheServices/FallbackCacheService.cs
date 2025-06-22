using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Table_Chair_Application.CacheServices
{
    public class FallbackCacheService : ICacheService
    {
        private readonly ICacheService _redisCache;
        private readonly ICacheService _memoryCache;

        public FallbackCacheService(ICacheService redisCache, ICacheService memoryCache)
        {
            _redisCache = redisCache;
            _memoryCache = memoryCache;
        }

        private async Task<T> TryRedis<T>(Func<ICacheService, Task<T>> redisFunc, Func<ICacheService, Task<T>> fallbackFunc)
        {
            try
            {
                return await redisFunc(_redisCache);
            }
            catch
            {
                return await fallbackFunc(_memoryCache);
            }
        }

        private async Task TryRedis(Func<ICacheService, Task> redisFunc, Func<ICacheService, Task> fallbackFunc)
        {
            try
            {
                await redisFunc(_redisCache);
            }
            catch
            {
                await fallbackFunc(_memoryCache);
            }
        }

        public Task SetStringAsync(string key, string value, TimeSpan? expiry = null) =>
            TryRedis(c => c.SetStringAsync(key, value, expiry), c => c.SetStringAsync(key, value, expiry));

        public Task<string?> GetStringAsync(string key) =>
            TryRedis(c => c.GetStringAsync(key), c => c.GetStringAsync(key));

        public Task<bool> KeyExistsAsync(string key) =>
            TryRedis(c => c.KeyExistsAsync(key), c => c.KeyExistsAsync(key));

        public Task RemoveAsync(string key) =>
            TryRedis(c => c.RemoveAsync(key), c => c.RemoveAsync(key));

        public Task<long> IncrementAsync(string key, TimeSpan? expiry = null) =>
            TryRedis(c => c.IncrementAsync(key, expiry), c => c.IncrementAsync(key, expiry));

        public Task KeyExpireAsync(string key, TimeSpan expiry) =>
            TryRedis(c => c.KeyExpireAsync(key, expiry), c => c.KeyExpireAsync(key, expiry));

        public Task ListLeftPushAsync(string key, string value) =>
            TryRedis(c => c.ListLeftPushAsync(key, value), c => c.ListLeftPushAsync(key, value));

        public Task<List<string>> ListRangeAsync(string key, int start = 0, int stop = -1) =>
            TryRedis(c => c.ListRangeAsync(key, start, stop), c => c.ListRangeAsync(key, start, stop));
    }
}

