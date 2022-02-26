using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Extensions.Middlewares.Basics
{
    /// <summary>
    /// 这个很少用到，业务异常都会被 GlobalExceptionFilter 拦截处理
    /// </summary>
    public class ExceptionHandlerMidd
    {
        private readonly RequestDelegate _next;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ExceptionHandlerMidd));

        public ExceptionHandlerMidd(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _log.Error(ex.GetBaseException().ToString());

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(MsgHelper.Fail<object>($"中间件异常拦截器：{ex.Message}", ex, code: StatusCodes.Status500InternalServerError)));
            }
        }
    }
}