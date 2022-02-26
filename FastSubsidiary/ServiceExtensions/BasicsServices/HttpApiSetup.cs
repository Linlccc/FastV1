using FastTool.WebApi.XS;
using Microsoft.Extensions.DependencyInjection;
using WebApiClient.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.BasicsServices
{
    public static class HttpApiSetup
    {
        public static void AddHttpApiSetup(this IServiceCollection services)
        {
            //写接口类继承 IHttpApi ，然后在这里注入
            //使用时直接注册然后调用指定接口就行了

            //services.AddHttpApi<IDoubanApi>().ConfigureHttpApiConfig(c =>
            //{
            //    c.HttpHost = new Uri("http://api.xiaomafeixiang.com/");
            //    c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            //});

            //services.AddHttpApi<ITestApi>().ConfigureHttpApiConfig(c =>
            //{ });

            services.AddHttpApi<IXSApi>().ConfigureHttpApiConfig(c =>
            {
                //c.HttpHost = new Uri("https://www.dvdspring.com/");
            });
        }
    }
}