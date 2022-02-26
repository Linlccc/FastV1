using Autofac;
using Extensions.EasyDevelop;
using Extensions.Middlewares;
using Extensions.ServiceExtensions;
using Extensions.ServiceExtensions.Authentication_Authorization;
using Extensions.ServiceExtensions.Autofacs;
using Extensions.ServiceExtensions.BasicsServices;
using Extensions.ServiceExtensions.Cache;
using Extensions.ServiceExtensions.EasyDevelop;
using Extensions.ServiceExtensions.ServiceInit;
using Extensions.ServiceExtensions.Special;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fast
{
    public class Startup
    {
        /// <summary>
        /// 环境
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// 服务容器
        /// </summary>
        private IServiceCollection _services;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // 添加服务
        public void ConfigureServices(IServiceCollection services)
        {
            _env.ServiceInit();                                 //初始化配置

            services.AddBasicsServicesSetup(_configuration);    //基础服务

            services.AddAuthentication_AuthorizationSetup();    //身份认证和权限认证

            services.AddCacheSetup();                           //缓存操作类

            services.AddSqlsugarSetup();                        //Sqlsugar 数据库操作

            services.AddEasyDevelopSetup();                     //便于开发的服务

            services.AddSpecialSetup();                         //一些比较特殊的服务（任务..）

            _services = services;
        }

        //Autofac的注册
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacServicesRegister());
            builder.RegisterModule(new AutofacMainRegister(typeof(Startup)));
        }

        // http请求管道
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseBeforeMidd(_env);    //需要放在前面的中间件

            app.UseContentMidd();       //内容中间件

            app.UseCustomMidd();        //自定义中间（不用太在意顺序的）

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //SignalR（通讯）的路由(将这个路径的请求映射到SignalR里)
                endpoints.MapHub<ChatHub>("/chathub").RequireCors(AppConfig.GetNode("Cors", "SignalRName"));//使用 SignalR 指定的跨域方案
            });

            //启动服务时要执行一次的命令
            app.OneRun(_services, _env, serviceProvider, typeof(Startup));
        }
    }
}