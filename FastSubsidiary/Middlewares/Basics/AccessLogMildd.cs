using Attributes.Tool;
using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace Extensions.Middlewares.Basics
{
    /// <summary>
    /// 访问日志
    /// </summary>
    public class AccessLogMildd
    {
        /// <summary>
        ///
        /// </summary>
        private readonly RequestDelegate _next;

        private readonly IUserInfo _user;
        private readonly ILogger<AccessLogMildd> _logger;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        public AccessLogMildd(RequestDelegate next, IUserInfo user, ILogger<AccessLogMildd> logger, IHubContext<ChatHub, IChatClient> hubContext, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _user = user;
            _logger = logger;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //（不是api || 访问日志忽略） 的不记录
            if (!context.Request.Path.Value.Contains("api") || context.GetAttribute<AccessLogIgnoreAttribute>().IsNNull())
            {
                await _next(context);
                return;
            }

            AccessLogInfo accessLogInfo = new();

            Stream originalBody = context.Response.Body;
            try
            {
                using MemoryStream ms = new();
                context.Response.Body = ms;
                Stopwatch workTime = await _next.GetWorkTimeAsync(context);

                //将响应流写回原始响应里
                ms.Position = 0;
                await ms.CopyToAsync(originalBody);

                //加载请求信息，记录日志
                RequestInfo(accessLogInfo, context, workTime);
                //发送实时日志
                if (MiddlewareInfo.SignalrRealTimeLog) await _hubContext.Clients.Group(ChatHub.GroupInfos.AdminGroup.GroupId.ToString()).AdminLog(LogFolderInfo.AccessFolder, false, accessLogInfo);
                LogHelper.WritrLog(LogFolderInfo.AccessFolder, accessLogInfo.ToString());
            }
            catch (Exception ex)
            {
                // 记录异常
                _logger.LogError(ex.Message + "" + ex.InnerException);
                throw;
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        /// <summary>
        /// 获取用户请求信息
        /// </summary>
        /// <param name="accessLogInfo">访问记录</param>
        /// <param name="context">http上下文</param>
        /// <param name="workTime">执行时间</param>
        /// <returns></returns>
        private void RequestInfo(AccessLogInfo accessLogInfo, HttpContext context, Stopwatch workTime)
        {
            //请求信息，用户信息
            accessLogInfo.UserId = _user.ID;
            accessLogInfo.UserName = _user.Name;
            accessLogInfo.Ip = context.GetClientIP();
            accessLogInfo.Agent = context.GetClientAgent();
            accessLogInfo.RequestMethod = context.Request.Method;
            accessLogInfo.RequestPath = HttpUtility.UrlDecode(context.Request.Path);
            accessLogInfo.RequestQuery = HttpUtility.UrlDecode(context.Request.QueryString.ToString());
            accessLogInfo.Week = DateTime.Now.GetWeek();
            //响应信息
            accessLogInfo.ResponseTime = DateTime.Now;
            accessLogInfo.WorkTime = workTime.ElapsedMilliseconds.ToString();

            //获取请求body(get,delete 没有body
            if (context.Request.Method != "GET" && context.Request.Method != "DELETE")
            {
                if (context.Request.ContentType != null && context.Request.ContentType.Contains("multipart/form-data")) accessLogInfo.RequestBody = $"上传了文件,上传内容大小 {context.Request.ContentLength} 字节(只是文件的大概大小，包含body信息)";
                else
                {
                    context.Request.Body.Position = 0;
                    using StreamReader requestBody = new(context.Request.Body);
                    accessLogInfo.RequestBody = requestBody.ReadToEnd();
                    context.Request.Body.Position = 0;
                }
            }
            //获取响应数据
            {
                context.Response.Body.Position = 0;
                using StreamReader responsebody = new(context.Response.Body);
                accessLogInfo.ResponseData = responsebody.ReadToEnd();
                context.Response.Body.Position = 0;
            }
        }
    }
}