using FastTool.GlobalVar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.Extensions.Configuration.AppConfig;

namespace Extensions.ServiceExtensions.ServiceInit
{
    public class AppConfigPrint
    {
        #region Table

        /// <summary>
        /// 表格的方式打印信息
        /// </summary>
        public static void AddAppConfigPrintingTable()
        {
            if (GetBoolNode("BasicConfig", "AppConfigAlert"))//应用配置警报
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.UTF8;//设置应用程序用于输出的编码

                #region 程序配置
                List<string[]> configInfos = new()
                {
                    new string[] { "当前环境", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") },
                    new string[] { "当前的授权方案", Authentication_AuthorizationInfo.UseAuthenticationSchemeName.OToString() },
                    new string[] { "任务调度", GetNode("StartServiceTimeExecuteOnce", "QuartzNetJob", "Enabled") },
                    new string[] { "RabbitMQ消息列队", GetNode("RabbitMQ", "Enabled") },
                    new string[] { "事件总线(必须开启消息列队)", GetNode("EventBus", "Enabled") },
                    new string[] { "服务注册与发现", GetNode("ConsulSetting", "Enabled") },
                    new string[] { "redis消息队列", GetNode("Redis", "RedisMq") },
                    new string[] { "是否多库", DBConfigInfo.IsMulti.OToString() },
                };

                new ConsoleTable()
                {
                    TitleString = "程序配置",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = configInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 4,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();
                #endregion 程序配置

                #region AOP
                List<string[]> aopInfos = new()
                {
                    new string[] { "Redis缓存AOP", AopInfo.RedisCachingAOP.OToString() },
                    new string[] { "内存缓存AOP", AopInfo.MemoryCachingAOP.OToString() },
                    new string[] { "数据库操作Aop", AopInfo.DbOperAOP.OToString() },
                    new string[] { "事务AOP", AopInfo.TranAOP.OToString() },
                    new string[] { "Sql执行AOP", AopInfo.SqlAOP.OToString() },
                };

                new ConsoleTable
                {
                    TitleString = "AOP",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = aopInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 7,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();
                #endregion AOP

                #region 中间件
                List<string[]> MiddlewareInfos = new()
                {
                    new string[] { "请求纪录中间件", MiddlewareInfo.UserAccessLog.OToString() },
                    new string[] { "SingnalR实时发送请求数据中间件", MiddlewareInfo.SignalrRealTimeLog.OToString() },
                    new string[] { "IP限流中间件", MiddlewareInfo.IpRateLimit.OToString() },
                    new string[] { "测试用户中间件", MiddlewareInfo.TestAuthUser.OToString() },
                };

                new ConsoleTable
                {
                    TitleString = "中间件",
                    Columns = new string[] { "配置名称", "配置信息/是否启动" },
                    Rows = MiddlewareInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 3,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
                Console.WriteLine();
                #endregion 中间件

                //数据库信息打印
                DBInfoPrintingTable();
            }
        }

        /// <summary>
        /// 表格的方式打印数据库信息
        /// </summary>
        public static void DBInfoPrintingTable()
        {
            #region 默认库
            List<string[]> defaultDbInfos = new()
            {
                new string[] { "写", DBConfigInfo.MutiConnectionString.Item1.ConnId, DBConfigInfo.MutiConnectionString.Item1.DbType.OToString(), DBConfigInfo.MutiConnectionString.Item1.Connection[0..20] + "..." },
            };
            int i = 0;
            DBConfigInfo.MutiConnectionString.Item1.SlaveLibraries.ForEach(s =>
            {
                defaultDbInfos.Add(new string[] { $"读{++i}", s.ConnId, "", s.Connection[0..20] + "..." });
            });

            new ConsoleTable
            {
                TitleString = "默认库",
                Columns = new string[] { "读/写", "链接ID", "类型", "连接字符" },
                Rows = defaultDbInfos,
                EnableCount = false,
                Alignment = Alignment.Left,
                ColumnBlankNum = 3,
                TableStyle = TableStyle.Alternative
            }.Writer(ConsoleColor.Blue);
            Console.WriteLine();
            #endregion 默认库

            #region 正常库
            List<string[]> normalDbInfos = new();
            i = 0;
            DBConfigInfo.MutiConnectionString.Item2.ForEach(m =>
            {
                normalDbInfos.Add(new string[] { $"写{++i}", m.ConnId, m.DbType.OToString(), m.Connection[0..20] + "..." });
                int j = 0;
                m.SlaveLibraries.ForEach(s =>
                {
                    normalDbInfos.Add(new string[] { $"读{++j}", m.ConnId, "", m.Connection[0..20] + "..." });
                });
                normalDbInfos.Add(new string[] { "", "", "", "" });
            });

            if (normalDbInfos.Count > 0)
                new ConsoleTable
                {
                    TitleString = "正常库",
                    Columns = new string[] { "读/写", "链接ID", "类型", "连接字符" },
                    Rows = normalDbInfos,
                    EnableCount = false,
                    Alignment = Alignment.Left,
                    ColumnBlankNum = 3,
                    TableStyle = TableStyle.Alternative
                }.Writer(ConsoleColor.Blue);
            Console.WriteLine();
            #endregion 正常库
        }

        #endregion Table
    }
}