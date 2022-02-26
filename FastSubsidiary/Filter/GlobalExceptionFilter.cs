using Extensions.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using StackExchange.Profiling;
using System;

namespace Extensions.Filter
{
    /// <summary>
    /// 全局异常日志，（只有发生异常才会进入这个筛选器）
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IUserInfo _userInfo;
        private readonly IUserClient _userClient;
        private const string _unableService = "Unable to resolve service for";

        public GlobalExceptionFilter(IWebHostEnvironment env, ILogger<GlobalExceptionFilter> logger, IUserInfo userInfo, IUserClient userClient)
        {
            _env = env;
            _logger = logger;
            _userInfo = userInfo;
            _userClient = userClient;
        }

        /// <summary>
        /// 全局异常
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (_userInfo.IsAuthenticated()) _userClient.SetColumn(u => u.ErrorCount == u.ErrorCount + 1, u => u.Id == _userInfo.ID.OToLong(0));

            Msg<Exception> errorMsg = new()
            {
                Message = "全局异常捕捉器：" + (context.Exception.Message.IsNNull() && context.Exception.Message.Contains(_unableService)
                ? context.Exception.Message.Replace(_unableService, $"（若新添加服务，需要重新编译项目）{_unableService}")
                : context.Exception.Message),
                Success = false,
                Code = StatusCodes.Status500InternalServerError
            };
            if (_env.IsDevelopment()) errorMsg.Data = context.Exception;//堆栈信息
            context.Result = new ObjectResult(errorMsg)
            {
                StatusCode = errorMsg.Code
            };

            //MiniProfiler添加错误报告
            string errInfo = CustomErrorLog(errorMsg.Message, context.Exception);
            MiniProfiler.Current.CustomTiming("Errors：", errInfo);

            //采用log4net 进行错误日志记录
            _logger.LogError(errorMsg.Message + errInfo);

            static string CustomErrorLog(string throwMsg, Exception ex) =>
                $"${Environment.NewLine}【自定义错误】：{throwMsg}" +
                $"${Environment.NewLine}【异常类型】：{ex.GetType().Name}" +
                $"${Environment.NewLine}【异常信息】：{ex.Message}" +
                $"${Environment.NewLine}【堆栈调用】：{ex.StackTrace}";
        }
    }
}