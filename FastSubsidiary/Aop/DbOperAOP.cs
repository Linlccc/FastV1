using Castle.DynamicProxy;
using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 数据库操作的AOP
    /// </summary>
    public class DbOperAOP : IInterceptor
    {
        private readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private readonly IUserInfo _userInfo;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public DbOperAOP(IUserInfo userInfo, IHubContext<ChatHub, IChatClient> hubContext)
        {
            _userInfo = userInfo;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public async void Intercept(IInvocation invocation)
        {
            //获取方法名
            string methodName = invocation.Method.Name;
            if (invocation.Method.IsGenericMethod) methodName += $"<{string.Join(',', invocation.Method.GetGenericArguments().Select(t => t.Name))}>";
            DbOperLogInfo dbOperLog = GetOperInfo(invocation);
            try
            {
                //性能检测当前步骤
                MiniProfiler.Current.Step($"执行 Repository 方法：{methodName}() -> ");

                Stopwatch sw = new();
                sw.Start();
                invocation.Proceed();//执行任务
                sw.Stop();

                //得到工作时间和结果
                dbOperLog.WorkTime = sw.ElapsedMilliseconds.ToString();
                //要忽略的返回类型
                List<Type> ignoreType = new() { typeof(ISqlSugarClient), typeof(IBaseClient<>) };
                if (ignoreType.Any(i => invocation.Method.ReturnType.FullName.StartsWith(i.FullName))) dbOperLog.Result = invocation.ReturnValue.ToString();
                else dbOperLog.Result = invocation.Method.IsAsync() ? Programme2(invocation) : JsonConvert.SerializeObject(invocation.ReturnValue, _jsonSettings);
            }
            catch (Exception ex)
            {
                //收录异常
                MiniProfiler.Current.CustomTiming("Errors：", ex.Message);

                if (dbOperLog.WorkTime.IsNull()) dbOperLog.WorkTime = "N/A";
                dbOperLog.Result = $"方法中出现异常：{ex.Message + ex.InnerException}";
            }
            finally
            {
                //发送实时日志
                if (MiddlewareInfo.SignalrRealTimeLog) await _hubContext.Clients.Group(ChatHub.GroupInfos.AdminGroup.GroupId.ToString()).AdminLog(LogFolderInfo.DbOperFolder, false, dbOperLog);
                LogHelper.WritrLog(LogFolderInfo.DbOperFolder, dbOperLog.ToString());
            }

            //获取操作信息
            DbOperLogInfo GetOperInfo(IInvocation invocation)
            {
                //获取参数
                string[] carryParameter = invocation.Arguments.Select(a => a == null ? "null" : a.GetType().IsEntrustOrExpression() ? a.ToString() : JsonConvert.SerializeObject(a ?? "", _jsonSettings)).ToArray();
                return new DbOperLogInfo()
                {
                    OperUserName = _userInfo.Name,
                    OperUserId = _userInfo.ID,
                    OperType = invocation.TargetType.Name,
                    OperMethod = methodName,
                    Parameters = string.Join(", ", carryParameter)
                };
            }
        }

        /// <summary>
        /// 获取异步结果方案1
        /// 反射获取属性的值
        /// </summary>
        public string Programme1(IInvocation invocation)
        {
            Type type = invocation.Method.ReturnType;
            if (type.IsNotResultAsync()) return "NULL";
            return JsonConvert.SerializeObject(type.GetProperty("Result").GetValue(invocation.ReturnValue), _jsonSettings);
        }

        /// <summary>
        /// 获取异步结果方案2【推荐】
        /// 直接动态类型获取到结果
        /// </summary>
        public string Programme2(IInvocation invocation)
        {
            if (invocation.Method.ReturnType.IsNotResultAsync()) return "NULL";
            object result = ((dynamic)invocation.ReturnValue).Result;
            return JsonConvert.SerializeObject(result, _jsonSettings);
        }
    }
}