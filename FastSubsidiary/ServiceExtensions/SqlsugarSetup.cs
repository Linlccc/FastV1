using FastTool.GlobalVar;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;

namespace Extensions.ServiceExtensions
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //把多个连接对象注入服务，这里必须采用Scoped，因为有事务操作（数据回滚）
            services.AddScoped<ISqlSugarClient>(serviceProvider =>
            {
                IHubContext<ChatHub, IChatClient> hubContext = default;
                if (MiddlewareInfo.SignalrRealTimeLog) hubContext = serviceProvider.GetService<IHubContext<ChatHub, IChatClient>>();
                List<ConnectionConfig> listConfig = new();   //链接配置信息

                // 加入默认库
                listConfig.Add(GetConnectionConfig(DBConfigInfo.MutiConnectionString.Item1, hubContext));

                //加入正常库
                DBConfigInfo.MutiConnectionString.Item2.ForEach(d =>
                {
                    listConfig.Add(GetConnectionConfig(d, hubContext));
                });

                return new SqlSugarClient(listConfig);
            });
        }

        /// <summary>
        /// 创建一个连接配置（基本就在注册服务时使用）
        /// </summary>
        /// <param name="mutiDB">连接信息</param>
        /// <returns></returns>
        private static ConnectionConfig GetConnectionConfig(MutiDBOperate mutiDB, IHubContext<ChatHub, IChatClient> hubContext)
        {
            //得到读库
            List<SlaveConnectionConfig> listConfig_Slave = new();
            mutiDB.SlaveLibraries.ForEach(s =>
            {
                listConfig_Slave.Add(new SlaveConnectionConfig() { HitRate = s.HitRate, ConnectionString = s.Connection });
            });

            AopEvents aopEvents = new();
            if (AopInfo.SqlAOP)
            {
                //执行前
                aopEvents.OnLogExecuting = async (sql, pars) =>
                {
                    SqlLogInfo sqlLogInfo = new(GetParameters(pars), sql);
                    string log = sqlLogInfo.ToString();
                    if (MiddlewareInfo.MiniProfiler) MiniProfiler.Current.CustomTiming("SqlSugar：", log);//性能检测
                    //发送实时日志
                    if (MiddlewareInfo.SignalrRealTimeLog) await hubContext?.Clients.Group(ChatHub.GroupInfos.AdminGroup.GroupId.ToString()).AdminLog(LogFolderInfo.SqlLogFolder, false, sqlLogInfo);
                    LogHelper.WritrLog(LogFolderInfo.SqlLogFolder, log);//记录日志
                };
                //报错
                aopEvents.OnError = async ex =>
                {
                    ErrorSqlLogInfo errorSqlLogInfo = new(GetParameters(ex.Parametres as SugarParameter[]), ex.Sql, ex.StackTrace);
                    string log = errorSqlLogInfo.ToString();
                    if (MiddlewareInfo.MiniProfiler) MiniProfiler.Current.CustomTiming("ErrorSqlSugar：", log);
                    //发送实时日志
                    if (MiddlewareInfo.SignalrRealTimeLog) await hubContext?.Clients.Group(ChatHub.GroupInfos.AdminGroup.GroupId.ToString()).AdminLog(LogFolderInfo.ErrorSqlLogFolder, false, errorSqlLogInfo);
                    LogHelper.WritrLog(LogFolderInfo.ErrorSqlLogFolder, log);
                };
#if false
                //执行完
                aopEvents.OnLogExecuted = (sql, pars) =>
                {
                };
                //可以修改sql和参数
                aopEvents.OnExecutingChangeSql = (sql, pars) =>
                {
                    return new KeyValuePair<string, SugarParameter[]>(sql, pars);
                };
                //差异日志
                aopEvents.OnDiffLogEvent = dl =>
                {
                    var editBeforeData = dl.BeforeData;//操作前记录  包含： 字段描述 列名 值 表名 表描述
                    var editAfterData = dl.AfterData;//操作后记录   包含： 字段描述 列名 值  表名 表描述
                    var sql = dl.Sql;
                    var parameter = dl.Parameters;
                    var data = dl.BusinessData;//这边会显示你传进来的对象
                    var time = dl.Time;//应该是执行时间
                    var diffType = dl.DiffType;//enum insert 、update and delete
                };
#endif
            }

            ConnectionConfig config = new()
            {
                ConfigId = mutiDB.ConnId.ToLower(),             //链接id
                ConnectionString = mutiDB.Connection,           //链接字符串
                DbType = (DbType)mutiDB.DbType,                 //数据库类型
                IsAutoCloseConnection = true,                   //是否自动关闭连接
                AopEvents = aopEvents,                          //数据库Aop
                MoreSettings = new ConnMoreSettings()           //数据库的更多连接设置
                {
                    IsAutoRemoveDataCache = true                //自动移除数据缓存
                },
                // 读库连接信息
                SlaveConnectionConfigs = listConfig_Slave,
            };

            return config;

            //得到执行的sql的信息
            static string GetParameters(SugarParameter[] pars)
            {
                string parameterStr = string.Empty;
                for (int i = 0; i < pars.Length; i++) parameterStr += $"{pars[i].ParameterName}:{pars[i].Value}" + (i == pars.Length - 1 ? "" : "|");
                return parameterStr;
            }
        }
    }
}