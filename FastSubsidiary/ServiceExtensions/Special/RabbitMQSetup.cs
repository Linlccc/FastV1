using EventBus.PersistentConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Extensions.ServiceExtensions.Special
{
    /// <summary>
    /// 注入 RabbitMQ 连接对象
    /// </summary>
    public static class RabbitMQSetup
    {
        public static void AddRabbitMQSetup(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(serviceProvider =>
            {
                ILogger<RabbitMQPersistentConnection> logger = serviceProvider.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                ConnectionFactory factory = new()
                {
                    HostName = AppConfig.GetNode("RabbitMQ", "HostName"),
                    Port = AppConfig.GetNode("RabbitMQ", "Port").OToInt(5672),
                    DispatchConsumersAsync = true
                };

                if (AppConfig.GetNode("RabbitMQ", "UserName").IsNNull()) factory.UserName = AppConfig.GetNode("RabbitMQ", "UserName");
                if (AppConfig.GetNode("RabbitMQ", "Password").IsNNull()) factory.Password = AppConfig.GetNode("RabbitMQ", "Password");

                int retryCount = AppConfig.GetNode("RabbitMQ", "RetryCount").OToInt(5);

                return new RabbitMQPersistentConnection(logger, factory, retryCount);
            });
        }
    }
}