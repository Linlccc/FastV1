using System;

namespace Microsoft.Extensions.Caching.Memory
{
    public class MemoryCaching : ICachingProvider
    {
        ////引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private readonly IMemoryCache _cache;

        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 从缓存获取数据
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <returns>值</returns>
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        /// <summary>
        /// 设置缓存,如果已有该缓存覆盖
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="cacheValue">值</param>
        /// <param name="Expiration">过期时间（秒）——默认7200（2小时）</param>
        public void Set(string cacheKey, object cacheValue, int Expiration = 7200)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(Expiration);
            _cache.Set(cacheKey, cacheValue, ts);
        }
    }
}