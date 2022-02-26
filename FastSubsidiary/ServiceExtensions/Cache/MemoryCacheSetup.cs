using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.Cache
{
    /// <summary>
    /// Memory缓存
    /// </summary>
    public static class MemoryCacheSetup
    {
        public static void AddMemoryCacheSetup(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache>(ServiceProvider => new MemoryCache(new MemoryCacheOptions()));//注入内存缓存
            services.AddScoped<ICachingProvider, MemoryCaching>();  //注入内容缓存操作，后面在需要使用内存缓存都使用这个
        }
    }
}