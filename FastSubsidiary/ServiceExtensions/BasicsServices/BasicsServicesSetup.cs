using Extensions.BasicsServices;
using Extensions.Filter;
using FastSubsidiary.ServiceExtensions.BasicsServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Extensions.ServiceExtensions.BasicsServices
{
    public static class BasicsServicesSetup
    {
        /// <summary>
        /// 基础服务注入
        /// </summary>
        /// <param name="service"></param>
        public static void AddBasicsServicesSetup(this IServiceCollection service, IConfiguration configuration)
        {
            _ = service ?? throw new ArgumentNullException(nameof(service));

            service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  //Http 上下文

            service.AddCorsSetup();                                             //跨域

            service.AddIpPolicyRateLimitSetup(configuration);                   //ip限流

            //实时通讯 防止乱码需要引入这个包  Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson
            service.AddSignalR().AddNewtonsoftJsonProtocol(configure =>
                {
                    configure.PayloadSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
                    configure.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;//采用本地时间
                    configure.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用
                    configure.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();//帕斯卡示大小写命名（首字母大写）
                });

            service.AddHttpApiSetup();                                          //注册WebApiClient接口,(使用第三方api等)

            //配置这两种方式下都io同步
            service.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            service.AddControllers(configure =>
            {
                //全局异常过滤
                configure.Filters.Add(typeof(GlobalExceptionFilter));
                //全局路由前缀，统一修改路由
                configure.Conventions.Add(new GlobalRoutePrefix());
                //Api 方法请求动作约定
                configure.Conventions.Add(new ApiControllerActionConvention());
                //自定义模型绑定（插入第一个）
                configure.ModelBinderProviders.Insert(0, new CustomModelBinderProviders());
            })
                //全局配置Json序列化处理
                .AddNewtonsoftJson(jsonOptions =>
                {
                    //忽略循环引用
                    jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //获取或设置将.NET对象序列化为JSON（反之亦然）时，序列化程序使用的协定解析器。--使用默认属性(Pascal)大小写
                    jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    //设置时间格式
                    jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    //设置本地时间而非UTC时间
                    jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    //忽略Model中为null的属性
                    //jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            service.AddModelStateErrorSetup();                                  //请求时模型验证错误自定义响应信息
        }
    }
}