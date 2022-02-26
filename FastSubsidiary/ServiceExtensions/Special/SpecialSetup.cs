using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ServiceExtensions.Special
{
    public static class SpecialSetup
    {
        /// <summary>
        /// 注入一些比较特殊的服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddSpecialSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddJobSetup();         //任务调度

            services.AddRabbitMQSetup();    //RabbitMQ 持久连接
            services.AddEventBusSetup();    //事件总线
        }
    }
}