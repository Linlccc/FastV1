using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SqlSugar;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Quartz.Listeners
{
    /// <summary>
    /// 任务调度监听器
    /// </summary>
    public class SchedulerListener : ISchedulerListener
    {
        private readonly IServiceProvider _serviceProvider;

        public SchedulerListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
        {
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            ITimedTaskClient timedTaskClient = serviceScope.ServiceProvider.GetService<ITimedTaskClient>();
            long id = jobDetail.Key.Name.OToLong();
            await timedTaskClient.SetColumnAsync(t => t.IsStart == true, t => t.Id == id);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            ITimedTaskClient timedTaskClient = serviceScope.ServiceProvider.GetService<ITimedTaskClient>();
            long id = jobKey.Name.OToLong();
            await timedTaskClient.SetColumnAsync(t => t.IsStart == false, t => t.Id == id);
        }

        /// <summary>
        /// 任务中断
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务暂停
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务恢复
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 计划的任务
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务组暂停
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务组恢复
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 计划外的任务
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务调度器错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cause"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 待机模式下的任务调度器
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务调度器关闭
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SchedulerShutdown(CancellationToken cancellationToken = default)
        {
            //关闭所有任务状态
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            ITimedTaskClient timedTaskClient = serviceScope.ServiceProvider.GetService<ITimedTaskClient>();
            await timedTaskClient.SetColumnAsync(t => t.IsStart == false, t => t.IsStart == true);

            Console.WriteLine("任务调度关闭！");
        }

        /// <summary>
        /// 任务调度器关闭中
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务调度器已启动
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("任务调度启动！");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务调度器启动中
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulerStarting(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 任务调度器数据清除
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器达到不在触发的条件（不在触发）
        /// 将任务状态设置成暂停
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            //这个任务已经达到不在触发的条件，修改任务状态
            IServiceScope serviceScope = _serviceProvider.CreateScope();
            ITimedTaskClient timedTaskClient = serviceScope.ServiceProvider.GetService<ITimedTaskClient>();
            long id = trigger.Key.Name.OToLong();
            await timedTaskClient.SetColumnAsync(t => t.IsStart == false, t => t.Id == id);
        }

        /// <summary>
        /// 触发器暂停
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器恢复
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器组暂停
        /// </summary>
        /// <param name="triggerGroup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器组恢复
        /// </summary>
        /// <param name="triggerGroup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}