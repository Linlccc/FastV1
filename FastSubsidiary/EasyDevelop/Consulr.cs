using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Extensions.Middlewares
{
    public static class Consulr
    {
        /// <summary>
        /// Consul 注册服务
        /// </summary>
        public static void InitConsul(IHostApplicationLifetime lifetime)
        {
            if (!AppConfig.GetBoolNode("ConsulSetting", "Enabled")) return;

            ConsulClient consulClient = new ConsulClient(configOverride =>
             {
                 configOverride.Address = new Uri(AppConfig.GetNode("ConsulSetting", "ConsulAddress"));    //Consul 服务的地址
             });

            //配置信息
            AgentServiceRegistration registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),//服务实例唯一标识
                Name = AppConfig.GetNode("ConsulSetting", "ServiceName"),//服务名
                Address = AppConfig.GetNode("ConsulSetting", "ServiceIP"), //服务IP
                Port = AppConfig.GetNode("ConsulSetting", "ServicePort").OToInt(),//服务端口
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                    HTTP = $"http://{AppConfig.GetNode("ConsulSetting", "ServiceIP")}:{AppConfig.GetNode("ConsulSetting", "ServicePort")}{AppConfig.GetNode("ConsulSetting", "ServiceHealthCheck")}",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)//超时时间
                }
            };

            //服务注册
            consulClient.Agent.ServiceRegister(registration).Wait();

            //应用程序终止时，取消注册
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }
    }
}