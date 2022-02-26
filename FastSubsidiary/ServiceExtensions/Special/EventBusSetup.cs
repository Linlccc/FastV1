using Autofac;
using EventBus.EventBus;
using EventBus.EventBusSubscriptions;
using EventBus.PersistentConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Extensions.ServiceExtensions.Special
{
    /// <summary>
    /// 事件总线服务
    /// </summary>
    public static class EventBusSetup
    {
        /// <summary>
        /// 添加事件总线服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddEventBusSetup(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();//事件总线订阅管理器
            services.AddSingleton<IEventBusr, RabbitMQEventBus>(serviceProvider =>
            {
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope iLifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
                ILogger<RabbitMQEventBus> logger = serviceProvider.GetRequiredService<ILogger<RabbitMQEventBus>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = serviceProvider.GetRequiredService<IEventBusSubscriptionsManager>();
                int retryCount = AppConfig.GetNode("RabbitMQ", "RetryCount").OToInt(5);

                return new RabbitMQEventBus(logger, rabbitMQPersistentConnection, iLifetimeScope, eventBusSubcriptionsManager, AppConfig.GetNode("EventBus", "SubscriptionClientName"), retryCount);
            });
        }
    }
}