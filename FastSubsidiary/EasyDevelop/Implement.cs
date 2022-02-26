using Extensions.Middlewares;
using Extensions.Middlewares.EasyDevelop;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;

namespace Extensions.EasyDevelop
{
    public static class Implement
    {
        /// <summary>
        /// 这几个只会执行一次,因为并不是中间件，是拦截路由或者，服务启动时执行任务，只有加入http管道的中间件才会每一次请求都执行
        /// </summary>
        /// <param name="app"></param>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="serviceProvider">服务容器</param>
        /// <param name="startupType">api服务程序集中的类类型</param>
        public static void OneRun(this IApplicationBuilder app, IServiceCollection services, IWebHostEnvironment env, IServiceProvider serviceProvider, Type startupType)
        {
            //查看注入的所有服务
            app.InitAllServices(services);

            //配置使用Swagger接口文档
            app.InitSwagger();

            //构建数据库，表，添加种子数据
            ISqlSugarClient sqlSugarClient = serviceProvider.GetService<ISqlSugarClient>();
            sqlSugarClient.InitSeedData(env.WebRootPath, startupType);

            //任务调度
            QuartzJob.InitQuartzJob(serviceProvider);

            //配置服务注册
            IHostApplicationLifetime lifetime = serviceProvider.GetService<IHostApplicationLifetime>();
            Consulr.InitConsul(lifetime);

            //订阅事件总线
            app.InitEventBus();
        }
    }
}