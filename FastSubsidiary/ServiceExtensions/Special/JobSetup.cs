using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Linq;
using Tasks.Quartz;

namespace Extensions.ServiceExtensions.Special
{
    /// <summary>
    /// 任务调度
    /// </summary>
    public static class JobSetup
    {
        public static void AddJobSetup(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerCenterServer, SchedulerCenterServer>();

            //直接注入所有任务类，就不用一个一个手动注入了
            Type baseType = typeof(IJob);//任务基类
            Type[] types = typeof(SchedulerCenterServer).Assembly.DefinedTypes  //得到任务程序集中的所有任务执行类
                .Select(t => t.AsType())
                .Where(t => t != baseType && baseType.IsAssignableFrom(t) && t.IsClass).ToArray();
            foreach (Type tpye in types) services.AddTransient(tpye);
        }
    }
}