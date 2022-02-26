using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.BasicsServices
{
    public static class IpPolicyRateLimitSetup
    {
        /// <summary>
        /// 限流
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddIpPolicyRateLimitSetup(this IServiceCollection services, IConfiguration configuration)
        {
            // 存储速率限制计数器和ip规则(缓存在本地)
            services.AddMemoryCache();
            //添加限流服务,从 appsettings.json 配置文件中读取规则
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            // 注入计数器和规则存储
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //注入计数器和规则分布式缓存存储
            //services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            // 配置（解析器，计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}