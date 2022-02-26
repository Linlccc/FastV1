using FastTool.GlobalVar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Yitter.IdGenerator;

namespace Extensions.ServiceExtensions.ServiceInit
{
    /// <summary>
    /// 服务启动初始化基本需求
    /// </summary>
    public static class ServiceInitSetup
    {
        public static void ServiceInit(this IWebHostEnvironment env)
        {
            _ = env ?? throw new ArgumentNullException(nameof(env));

            LogHelper.ContentRoot = env.ContentRootPath;    //配置日志文件（文件存放的路劲）
            FileWellFolderInfo.WebRootPath = env.WebRootPath;//配置wwwroot路径

            AppConfigPrint.AddAppConfigPrintingTable();   //打印配置信息
            //雪花ID配置
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions()
            {
                Method = AppConfig.GetNode("YitId", "Method").OToInt16(),
                WorkerId = AppConfig.GetNode("YitId", "WorkerId").OToUInt16(),
                TopOverCostCount = AppConfig.GetNode("YitId", "TopOverCostCount").OToInt()
            });
        }
    }
}