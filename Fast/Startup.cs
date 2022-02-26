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
        /// ����
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// ��������
        /// </summary>
        private IServiceCollection _services;

        /// <summary>
        /// ����
        /// </summary>
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // ��ӷ���
        public void ConfigureServices(IServiceCollection services)
        {
            _env.ServiceInit();                                 //��ʼ������

            services.AddBasicsServicesSetup(_configuration);    //��������

            services.AddAuthentication_AuthorizationSetup();    //�����֤��Ȩ����֤

            services.AddCacheSetup();                           //���������

            services.AddSqlsugarSetup();                        //Sqlsugar ���ݿ����

            services.AddEasyDevelopSetup();                     //���ڿ����ķ���

            services.AddSpecialSetup();                         //һЩ�Ƚ�����ķ�������..��

            _services = services;
        }

        //Autofac��ע��
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacServicesRegister());
            builder.RegisterModule(new AutofacMainRegister(typeof(Startup)));
        }

        // http����ܵ�
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseBeforeMidd(_env);    //��Ҫ����ǰ����м��

            app.UseContentMidd();       //�����м��

            app.UseCustomMidd();        //�Զ����м䣨����̫����˳��ģ�

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //SignalR��ͨѶ����·��(�����·��������ӳ�䵽SignalR��)
                endpoints.MapHub<ChatHub>("/chathub").RequireCors(AppConfig.GetNode("Cors", "SignalRName"));//ʹ�� SignalR ָ���Ŀ��򷽰�
            });

            //��������ʱҪִ��һ�ε�����
            app.OneRun(_services, _env, serviceProvider, typeof(Startup));
        }
    }
}