using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// httpContext 拓展
    /// </summary>
    public static class HttpContextExtension
    {
        /// <summary>
        /// 获取客户端 Ip
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <returns>客户端ip</returns>
        public static string GetClientIP(this HttpContext context)
        {
            string ip = context.Request.Headers["X-Forwarded-For"].OToString();
            if (ip.IsNull()) ip = context.Connection.RemoteIpAddress.OToString();

            return ip;
        }

        /// <summary>
        /// 获取客户端 Agent
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientAgent(this HttpContext context) => context.Request.Headers["User-Agent"].OToString();

        /// <summary>
        /// 获取 HTTP请求处理 工作时间
        /// </summary>
        /// <param name="next">http 处理函数</param>
        /// <returns></returns>
        public static async Task<Stopwatch> GetWorkTimeAsync(this RequestDelegate next, HttpContext context)
        {
            Stopwatch sw = new();
            sw.Restart();
            await next(context);
            sw.Stop();
            return sw;
        }

        /// <summary>
        /// 从http请求中获取控制器和方法信息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static ControllerActionDescriptor GetControllerActionDescriptor(this HttpContext httpContext) => httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        /// <summary>
        /// 获取指定特性
        /// </summary>
        /// <typeparam name="T">指定特性类型</typeparam>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this HttpContext httpContext) where T : class => httpContext.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<T>();
    }
}