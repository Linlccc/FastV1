using AspNetCoreRateLimit;
using Extensions.Middlewares.Basics;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace Extensions.Middlewares
{
    /// <summary>
    /// http 管道帮助类
    /// </summary>
    public static class MiddlewareHelpers
    {
        #region 中间件使用集

        /// <summary>
        /// 放在前面的中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBeforeMidd(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            //限流（放在前面有较好的）
            app.UseIpLimitMildd();

            //判断环境
            if (env.IsDevelopment())
            {
                //默认错误输出（这个一般不会触发，都被 异常中间件，全局异常捕捉 处理了）
                app.UseDeveloperExceptionPage();
                //自定义异常拦截中间件
                app.UseExceptionHandlerMidd();
            }
            else
            {
                //异常处理
                app.UseExceptionHandler("/Error");
                //拦截异常处理
                app.Map("/Error", builder => builder.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(MsgHelper.Fail($"服务器处理错误,请联系管理员", "^-^", StatusCodes.Status500InternalServerError)));
                }));

                // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
                // 强制实施 HTTPS 在 ASP.NET Core,使用以下两个中间件 配合
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            return app;
        }

        /// <summary>
        /// 内容中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseContentMidd(this IApplicationBuilder app)
        {
            //使用静态文件，默认wwwroot
            app.UseStaticFiles();
            //使用cookie
            app.UseCookiePolicy();
            // 返回错误码(请求在400-599之间显示请求错误,可指定内容)
            app.UseStatusCodePages();
            //路由
            app.UseRouting();
            //启动请求内容可多次读取
            app.Use(async (httpContext, next) =>
            {
                httpContext.Request.EnableBuffering();

                await next.Invoke();
            });

            //请求本地化（必须在cors之前）【还需要了解作用，这几个缓存先放在这，后面了解】
            //app.UseRequestLocalization();

            //使用跨域策略
            app.UseCors(AppConfig.GetNode("Cors", "DefaultName"));

            //测试用户（放在认证前）
            app.UseTestAuthUser();
            //先开启认证（明确身份）
            app.UseAuthentication();
            //然后是授权（明确权限）
            app.UseAuthorization();

            //响应压缩【还需要了解作用】
            //app.UseResponseCompression();
            //响应缓存（必须在cors后面）【还需要了解作用】
            //app.UseResponseCaching();

            return app;
        }

        /// <summary>
        /// 自定义中间件
        /// 大多数自定义中间件放在这里面，不用怎么在乎顺序
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomMidd(this IApplicationBuilder app)
        {
            // 用户访问记录（要在认证后面）
            app.UseAccessLogMildd();

            //使用性能检测，尽量放在后面
            app.UseMiniProfile();

            return app;
        }

        #endregion 中间件使用集

        /// <summary>
        /// 记录访问日志中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAccessLogMildd(this IApplicationBuilder app) => MiddlewareInfo.UserAccessLog ? app.UseMiddleware<AccessLogMildd>() : app;

        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlerMidd(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMidd>();

        /// <summary>
        /// 测试用户避免频繁登录
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTestAuthUser(this IApplicationBuilder app) => MiddlewareInfo.TestAuthUser ? app.UseMiddleware<AuthMidd>() : app;

        /// <summary>
        /// 使用 Ip 限流
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseIpLimitMildd(this IApplicationBuilder app) => MiddlewareInfo.IpRateLimit ? app.UseIpRateLimiting() : app;

        /// <summary>
        /// 使用 性能检测
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseMiniProfile(this IApplicationBuilder app) => MiddlewareInfo.MiniProfiler ? app.UseMiniProfiler() : app;
    }
}