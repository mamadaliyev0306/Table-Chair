using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.CacheServices
{
    public interface ICacheService
    {
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task RemoveAsync(string key);
        Task<long> IncrementAsync(string key, TimeSpan? expiry = null);
        Task KeyExpireAsync(string key, TimeSpan expiry);
        Task ListLeftPushAsync(string key, string value);
        Task<List<string>> ListRangeAsync(string key, int start = 0, int stop = -1);

    }
}
