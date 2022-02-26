using Extensions.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ServiceExtensions.EasyDevelop
{
    public static class EasyDevelopSetup
    {
        /// <summary>
        /// 便于开发的服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddEasyDevelopSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSwaggerSetup();         //api文档

            services.AddMiniProfilerSetup();    //性能检测

            services.AddAutoMapper(typeof(AutoMapperConfig));   //类直接自动映射
        }
    }
}