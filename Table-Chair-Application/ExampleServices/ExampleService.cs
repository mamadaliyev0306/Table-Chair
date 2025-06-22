using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.CacheServices;

namespace Table_Chair_Application.ExampleServices
{
    public class ExampleService
    {
        private readonly ICacheService _cacheService;

        public ExampleService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task StoreExampleAsync()
        {
            await _cacheService.SetStringAsync("key", "value", TimeSpan.FromMinutes(10));
        }

        public async Task<string?> GetExampleAsync()
        {
            return await _cacheService.GetStringAsync("key");
        }
    }

}
