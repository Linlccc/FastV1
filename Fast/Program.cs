using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Fast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder host = CreateHostBuilder(args);

            // 生成承载 web 应用程序的 Microsoft.AspNetCore.Hosting.IWebHost.Build 是WebHostBuilder最终的目的，将返回一个构造的WebHost，最终生成宿主
            IHost build = host.Build();
            //配置程序根服务
            build.InitService();
            // 运行 web 应用程序并阻止调用线程, 直到主机关闭
            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // 初始化默认主机构建器
            Host.CreateDefaultBuilder(args)
            // 配置应用程序 自定义添加配置信息
            .ConfigureAppConfiguration(AppConfig.AddConfigureFiles)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//使用 Autofac 的依赖注入
                                                                           // 配置日志
            .ConfigureLogging((hostingContext, builder) =>
            {
                //这里log4net 对默认的日志接管了
                builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
            })
            //配置 web主机 默认值
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                //配置web程序监听的url也可以在launchSettings中设置
                .UseUrls("http://*:8082");
            });
    }
}