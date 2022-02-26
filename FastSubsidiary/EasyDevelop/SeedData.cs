using log4net;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;

namespace Extensions.Middlewares.EasyDevelop
{
    public static class SeedData
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SeedData));

        public static void InitSeedData(this ISqlSugarClient sqlSugar, string webRootPath, Type startupType)
        {
            _ = sqlSugar ?? throw new ArgumentNullException(nameof(sqlSugar));

            try
            {
                bool isCreateDb = AppConfig.GetBoolNode("DBConfig", "CreateDb");//是否创建数据库
                bool isUpdateDb = AppConfig.GetBoolNode("DBConfig", "UpdateDb");//是否更新数据库表
                bool isSeedData = AppConfig.GetBoolNode("DBConfig", "SeedData");//是否添加种子数据

                if (isCreateDb || isUpdateDb || isSeedData) //只要有一样要做就要去创建表
                {
                    sqlSugar.ReadySeed().Wait();
                }
            }
            catch (Exception e)
            {
                _log.Error($"添加种子数据时错误{Environment.NewLine + e.Message}");
                throw;
            }
        }
    }
}