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

            // ���ɳ��� web Ӧ�ó���� Microsoft.AspNetCore.Hosting.IWebHost.Build ��WebHostBuilder���յ�Ŀ�ģ�������һ�������WebHost��������������
            IHost build = host.Build();
            //���ó��������
            build.InitService();
            // ���� web Ӧ�ó�����ֹ�����߳�, ֱ�������ر�
            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // ��ʼ��Ĭ������������
            Host.CreateDefaultBuilder(args)
            // ����Ӧ�ó��� �Զ������������Ϣ
            .ConfigureAppConfiguration(AppConfig.AddConfigureFiles)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())//ʹ�� Autofac ������ע��
                                                                           // ������־
            .ConfigureLogging((hostingContext, builder) =>
            {
                //����log4net ��Ĭ�ϵ���־�ӹ���
                builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
            })
            //���� web���� Ĭ��ֵ
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                //����web���������urlҲ������launchSettings������
                .UseUrls("http://*:8082");
            });
    }
}