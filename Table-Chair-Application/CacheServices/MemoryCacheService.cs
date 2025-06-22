using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.CacheServices
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            _cache.Set(key, value, expiry ?? TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        public  Task<string?> GetStringAsync(string key)
        {
             _cache.TryGetValue(key, out string? value);
            return Task.FromResult(value);
        }

        public Task<bool> KeyExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public  Task RemoveAsync(string key)
        {
           _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<long> IncrementAsync(string key, TimeSpan? expiry = null)
        {
            var value = _cache.GetOrCreate<long>(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromHours(1);
                return 0;
            });

            value++;
            _cache.Set(key, value, expiry ?? TimeSpan.FromHours(1));
            return Task.FromResult(value);
        }
        public Task KeyExpireAsync(string key, TimeSpan expiry)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                _cache.Set(key, value, expiry);
            }

            return Task.CompletedTask;
        }
        private readonly Dictionary<string, List<string>> _listCache = new();

        public Task ListLeftPushAsync(string key, string value)
        {
            if (!_listCache.ContainsKey(key))
                _listCache[key] = new List<string>();

            _listCache[key].Insert(0, value);
            return Task.CompletedTask;
        }

        public Task<List<string>> ListRangeAsync(string key, int start = 0, int stop = -1)
        {
            if (!_listCache.TryGetValue(key, out var list))
                return Task.FromResult(new List<string>());

            if (stop == -1 || stop >= list.Count)
                stop = list.Count - 1;

            var result = list.Skip(start).Take(stop - start + 1).ToList();
            return Task.FromResult(result);
        }

    }

}
