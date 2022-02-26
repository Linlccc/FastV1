using Attributes.Entity;
using Attributes.SqlSugar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Model.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public static class DBSeed
    {
        /// <summary>
        /// 所有实体类 类型
        /// </summary>
        private static List<Type> _modelTypes = new();

        /// <summary>
        /// 准备种子
        /// </summary>
        /// <param name="sqlSugar"></param>
        /// <returns></returns>
        public static async Task ReadySeed(this ISqlSugarClient sqlSugar)
        {
            //得到所有的数据库实体类
            _modelTypes = (from t in typeof(RootEntity).Assembly.GetTypes()
                           where t.CheckAttribute<EntityAttribute>(false)
                           select t).ToList();

            //主库
            sqlSugar.CreateOrUpdateDBTabler(DBConfigInfo.DefaultDbID);

            //循环设置每一个 正常库
            foreach (MutiDBOperate mutiDB in DBConfigInfo.MutiConnectionString.Item2)
            {
                sqlSugar.CreateOrUpdateDBTabler(mutiDB.ConnId);
            }
        }

        private static void CreateOrUpdateDBTabler(this ISqlSugarClient sqlSugar, string connid)
        {
            (sqlSugar as SqlSugarClient).ChangeDatabase(connid.ToLower());//切换到当前要操作的库
            string currentConnid = sqlSugar.CurrentConnectionConfig.ConfigId;//当前数据库的连接id

            ConsoleTable dbTable = new()
            {
                TitleString = $"{currentConnid}  数据库操作",
                Columns = new string[] { "操作名", "操作结果" },
                ColumnBlankNum = 4,
                Alignment = Alignment.Left,
                EnableCount = false,
                TableStyle = TableStyle.Alternative,
                ColumnWides = new List<int>() { 28, 38 }
            };
            ConsoleHelper.WriteColorLine(dbTable.GetHeader(), ConsoleColor.Blue);

            sqlSugar.DbMaintenance.CreateDatabase();
            ConsoleHelper.WriteInfoLine(dbTable.GetNewRow(new string[] { "添加数据库", "Success" }));
            ConsoleHelper.WriteInfoLine(dbTable.GetNewRow(new string[] { "", "" }));

            //要操作的实体类型
            List<Type> entityTypes = new();
            //单库加载全部表
            //有 SugarConnectAttribute 特性并且标记是当前库
            //没有 SugarTable 特性，并且当前是默认库
            entityTypes = _modelTypes.Where(t =>
                  !DBConfigInfo.IsMulti
                  || t.CheckAttribute(false, out ConnectAttribute sugarConnect) && sugarConnect.Connid.ToLower() == currentConnid.ToLower()
                  || (!t.CheckAttribute<ConnectAttribute>(false) && currentConnid.ToLower() == DBConfigInfo.DefaultDbID.ToLower()))
                .ToList();

            foreach (Type type in entityTypes)
            {
                string tableName = type.CheckAttribute(false, out SugarTable sugarTable) ? sugarTable.TableName : type.Name;
                if (!sqlSugar.DbMaintenance.IsAnyTable(tableName) || AppConfig.GetBoolNode("DBConfig", "UpdateDb"))
                {
                    sqlSugar.CodeFirst.InitTables(type);
                    ConsoleHelper.WriteInfoLine(dbTable.GetNewRow(new string[] { $"添加/更新 {tableName}", "Success" }));
                }
            }
            ConsoleHelper.WriteInfoLine(dbTable.GetNewRow(new string[] { "---表操作完成---", "----如要更新表,请在appsettings中开启--" }));
            ConsoleHelper.WriteInfoLine(dbTable.GetEnd());
            Console.WriteLine();
        }

        /// <summary>
        /// 获取这个方法的请求url
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetActionUrl(MethodInfo method)
        {
            string urlInfo = string.Empty;
            if (method.DeclaringType.CheckAttribute(false, out RouteAttribute routeAttribute))//如果类有路由特性
                urlInfo = routeAttribute.Template.Replace("[controller]", method.DeclaringType.Name.Replace("Controller", "")).Replace("[action]", method.Name);
            else
                urlInfo = $"{method.DeclaringType.Name}/{method.Name}";

            //如果方法还有自己的路由后续
            if (method.CheckAttribute(true, out HttpMethodAttribute http) && http.Template.IsNNull()) urlInfo += "/" + http.Template;
            if (method.CheckAttribute(true, out RouteAttribute route) && route.Template.IsNNull()) urlInfo += "/" + route.Template;

            //判断第一个字符
            if (!urlInfo.StartsWith("/")) urlInfo = $"/{urlInfo}";

            //替换所有的路由参数
            MatchCollection matchs = Regex.Matches(urlInfo, @"{.+?}");
            foreach (Match m in matchs)
                urlInfo = urlInfo.Replace(m.Value, "[^/]*");

            return urlInfo;
        }
    }
}