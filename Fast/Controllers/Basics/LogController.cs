using Attributes.Tool;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 日志
    /// </summary>
    [ApiController, CustomRoute, AccessLogIgnore]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public LogController(ILogger<LogController> logger, IHubContext<ChatHub, IChatClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="count">日志条数</param>
        /// <returns></returns>
        public Msg<object> GetLogs(string logType, DateTime startTime, DateTime endTime, int count = 50)
        {
            //按照日志类型获取日志
            object result = logType switch
            {
                LogFolderInfo.AccessFolder => LogHelper.GetLogInfos<AccessLogInfo>(startTime, endTime, count),//访问日志
                LogFolderInfo.DbOperFolder => LogHelper.GetLogInfos<DbOperLogInfo>(startTime, endTime, count),
                LogFolderInfo.SqlLogFolder => LogHelper.GetLogInfos<SqlLogInfo>(startTime, endTime, count),
                LogFolderInfo.ErrorSqlLogFolder => LogHelper.GetLogInfos<ErrorSqlLogInfo>(startTime, endTime, count),
                _ => new List<object>()
            };
            return MsgHelper.Success(result);
        }
    }
}