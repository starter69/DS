using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers.Cache
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        //CacheKey
        //DataSourceName + SubProjectID
        private readonly IMemoryCache memoryCache;
        public CacheController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        //Reach datasource from the cache
        [HttpGet("{key}")]
        public DataTable GetCache(string key)
        {
            DataTable csvContent = new DataTable();
            memoryCache.TryGetValue(key, out csvContent);
            return csvContent;
        }
        //Send datasource csv file to cashe
        [HttpPost]
        public IActionResult SetCache(CacheRequest data)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(15),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromMinutes(15),
                Size = 1024,
            };
            memoryCache.Set(data.key, data.value, cacheExpiryOptions);
            return Ok();
        }
        public class CacheRequest
        {
            public string key { get; set; }
            public DataTable value { get; set; }
        }
    }
}
