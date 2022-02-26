using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using Tasks.Quartz;

namespace Extensions.Middlewares
{
    public static class QuartzJob
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(QuartzJob));

        /// <summary>
        /// 启动任务计划
        /// </summary>
        /// <param name="taskInfoDb"></param>
        /// <param name="schedulerCenter"></param>
        public static async void InitQuartzJob(IServiceProvider serviceProvider)
        {
            try
            {
                if (!AppConfig.GetBoolNode("StartServiceTimeExecuteOnce", "QuartzNetJob", "Enabled")) return;

                ITimedTaskClient tasksInfoDb = serviceProvider.GetService<ITimedTaskClient>();
                ISchedulerCenterServer schedulerCenter = serviceProvider.GetService<ISchedulerCenterServer>();

                List<TimedTask> tasksInfos = await tasksInfoDb.QueryAsync(t => t.IsStart);
                foreach (TimedTask ti in tasksInfos)
                {
                    if ((await schedulerCenter.StartJobAsync(ti)).Success) ConsoleHelper.WriteSuccessLine($"【{ti.Name}】 启动成功");
                    else ConsoleHelper.WriteErrorLine($"【{ti.Name}】 启动失败");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                _log.Error($"启动作业服务时报告了错误.{Environment.NewLine + ex.Message}");
                throw;
            }
        }
    }
}