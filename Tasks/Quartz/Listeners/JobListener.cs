using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using Quartz;
using SqlSugar;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Quartz.Listeners
{
    /// <summary>
    /// 任务执行监听器
    /// </summary>
    public class JobListener : IJobListener
    {
        private readonly IServiceProvider _serviceProvider;

        public JobListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name => "default";

        /// <summary>
        /// 任务被取消
        /// ITriggerListener 取消了执行时触发
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务执行前
        /// 如果 ITriggerListener 取消了执行将不会触发
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务执行后
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            // 打印任务运行时间
            JobDataMap data = context.JobDetail.JobDataMap;
            string Name = data.GetString("JobName");
            Console.WriteLine($"【{DateTime.Now:yyyy-MM-dd HH;mm:ss}】执行任务：【{Name}】用时：{context.JobRunTime.TotalMilliseconds}ms");

            //修改任务运行次数及日志
            long id = context.JobDetail.Key.Name.OToLong();
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            ITimedTaskClient timedTaskClient = serviceScope.ServiceProvider.GetService<ITimedTaskClient>();
            TimedTask tasksInfo = await timedTaskClient.QueryByIdAsync(id);//查询出任务
            if (tasksInfo != null)
            {
                tasksInfo.RunCounts++;
                tasksInfo.Log ??= "";
                string history = string.Join("<br/>", tasksInfo.Log.Split("<br/>").ToList().Where(s => s.IsNNull()).Take(5).ToArray());
                tasksInfo.Log = $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】【执行成功】<br/>" + history;
                await timedTaskClient.UpdateAsync(tasksInfo);//更新任务信息
            }
        }
    }
}