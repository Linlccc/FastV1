using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace Tasks.Quartz
{
    /// <summary>
    /// IJobFactory 是应用程序通过某种特殊机制生成 Quartz.IJob 实例（IJob的实现类完成任务）
    /// </summary>
    public class JobFactory : IJobFactory
    {
        /// <summary>
        /// 获取服务的对象
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 由调度程序在触发触发器时调用，在 IJob 的实例上调用 Execute 方法
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                IServiceScope serviceScope = _serviceProvider.CreateScope();
                IJob job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 如果需要，允许作业工厂销毁/清理作业。
        /// </summary>
        /// <param name="job"></param>
        public void ReturnJob(IJob job)
        {
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}