using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.CacheServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var result = await _db.StringGetAsync(key);
            return result.HasValue ? result.ToString() : null;
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<long> IncrementAsync(string key, TimeSpan? expiry = null)
        {
            var value = await _db.StringIncrementAsync(key);
            if (value == 1 && expiry != null)
                await _db.KeyExpireAsync(key, expiry);
            return value;
        }
        public async Task KeyExpireAsync(string key, TimeSpan expiry)
        {
            await _db.KeyExpireAsync(key, expiry);
        }
        public async Task ListLeftPushAsync(string key, string value)
        {
            await _db.ListLeftPushAsync(key, value);
        }

        public async Task<List<string>> ListRangeAsync(string key, int start = 0, int stop = -1)
        {
            var redisValues = await _db.ListRangeAsync(key, start, stop);
            return redisValues.Select(x => x.ToString()).ToList();
        }

    }

}
