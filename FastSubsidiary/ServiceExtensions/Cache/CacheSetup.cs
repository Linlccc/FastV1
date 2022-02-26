using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ServiceExtensions.Cache
{
    public static class CacheSetup
    {
        /// <summary>
        /// 注入缓存操作类
        /// </summary>
        /// <param name="services"></param>
        public static void AddCacheSetup(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddMemoryCacheSetup(); //Memory 缓存 (本地缓存)
            services.AddRedisCacheSetup();  //Redis 缓存，消息列队
        }
    }
}