using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.ServiceExtensions.BasicsServices
{
    /// <summary>
    /// 跨域
    /// </summary>
    public static class CorsSetup
    {
        /// <summary>
        /// 添加跨域策略
        /// </summary>
        /// <param name="services"></param>
        public static void AddCorsSetup(this IServiceCollection services)
        {
            services.AddCors(coreOptions =>
            {
                List<CorsInfo> corsInfos = AppConfig.GetNode<List<CorsInfo>>("Cors", "Rules").Where(c => c.Name.IsNNull()).ToList();

                corsInfos.ForEach(c =>
                {
                    coreOptions.AddPolicy(c.Name, policy =>
                     {
                         //如果是 signalR 的方案，单独处理
                         if (c.Name == AppConfig.GetNode("Cors", "SignalRName"))
                         {
                             //使用允许的来源和允许凭证（连接 signalR 必须）
                             policy.WithOrigins(c.AllowOrigins).AllowCredentials();
                         }
                         else
                         {
                             if (c.AllowAnyOrigin) policy.AllowAnyOrigin();
                             else policy.WithOrigins(c.AllowOrigins);
                         }

                         policy.AllowAnyMethod()                    //确保该策略允许任何方法
                            .AllowAnyHeader();                      //确保该策略允许任何标头
                     });
                });
            });
        }
    }

    /// <summary>
    /// 跨域配置信息
    /// </summary>
    public class CorsInfo
    {
        /// <summary>
        /// 跨域策略名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 允许任何来源
        /// </summary>
        public bool AllowAnyOrigin { get; set; }

        /// <summary>
        /// 允许的来源
        /// </summary>
        public string[] AllowOrigins { get; set; }
    }
}