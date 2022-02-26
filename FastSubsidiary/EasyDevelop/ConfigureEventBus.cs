using Attributes.EventBus;
using EventBus.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.EasyDevelop
{
    public static class ConfigureEventBus
    {
        public static void InitEventBus(this IApplicationBuilder app)
        {
            if (AppConfig.GetBoolNode("RabbitMQ", "Enabled") && AppConfig.GetBoolNode("EventBus", "Enabled"))
            {
                IEventBusr eventBus = app.ApplicationServices.GetRequiredService<IEventBusr>();

                ConsoleHelper.WriteInfoLine("************ 事件总线订阅 *****************");

                //得到所有要订阅的处理程序类型
                List<Type> SubscribeHandlers = typeof(ConfigureEventBus).Assembly.GetTypes().Where(t => t.CheckAttribute<SubscribeAttribute>(false)).ToList();
                SubscribeHandlers.ForEach(t =>
                {
                    //得到该处理程序类型中的所有要订阅的 路由
                    List<string> routeKeys = new();
                    t.GetMethods().ToList().ForEach(m => routeKeys.AddRange(m.GetRouteKeyByMethod()));
                    routeKeys.ForEach(route =>
                    {
                        eventBus.Subscribe(t, route);
                        ConsoleHelper.WriteSuccessLine($"使用 {t} 处理类型订阅了 {route} 路由");
                    });//在处理类型中订阅所有路由
                });
                eventBus.Ready();//可以开始接收消息了
                ConsoleHelper.WriteInfoLine("************ 事件【完】 *****************");
                Console.WriteLine();
            }
        }
    }
}