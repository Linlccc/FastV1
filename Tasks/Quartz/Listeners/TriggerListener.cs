using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Quartz.Listeners
{
    /// <summary>
    /// 调度器监听器
    /// </summary>
    public class TriggerListener : ITriggerListener
    {
        private readonly IServiceProvider _serviceProvider;

        public TriggerListener(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name => "default";

        /// <summary>
        /// 触发器完成
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="triggerInstructionCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器触发
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 触发器失火(这个每次调用任务都在执行，暂时不太理解)
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 是否要取消这个任务的执行
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}