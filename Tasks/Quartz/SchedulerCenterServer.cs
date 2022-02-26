using Model.BaseModels;
using Model.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Tasks.Quartz.Listeners;

namespace Tasks.Quartz
{
    public class SchedulerCenterServer : ISchedulerCenterServer
    {
        private readonly IJobFactory _jobFactory;
        private readonly IServiceProvider _serviceProvider;
        private IScheduler _scheduler;  //调度器

        #region 构造函数

        public SchedulerCenterServer(IJobFactory jobFactory, IServiceProvider serviceProvider)
        {
            _jobFactory = jobFactory;
            _serviceProvider = serviceProvider;
            GetSchedulerAsync();
        }

        /// <summary>
        /// 获取调度器
        /// </summary>
        /// <returns></returns>
        private void GetSchedulerAsync(bool isRe = false)
        {
            //有值 && 不是重置
            if (_scheduler != null && !isRe) return;

            // 从Factory中获取Scheduler实例
            NameValueCollection collection = new()
            {
                { "quartz.serializer.type", "binary" },
            };
            StdSchedulerFactory factory = new(collection);
            _scheduler = factory.GetScheduler().Result;

            _scheduler.ListenerManager.AddJobListener(new JobListener(_serviceProvider));//添加任务执行监听器
            _scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener(_serviceProvider));//调度程序监听器
            _scheduler.ListenerManager.AddTriggerListener(new TriggerListener(_serviceProvider));//触发器监听
        }

        #endregion 构造函数

        #region 管理任务

        /// <summary>
        /// 添加一个计划任务（映射程序集指定IJob实现类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> StartJobAsync(TimedTask sysSchedule)
        {
            if (sysSchedule == null) return MsgHelper.Fail<bool>("任务计划不存在");

            try
            {
                //确定调度程序中是否已存在具有给定标识符的 IJob
                JobKey jobKey = new(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                if (await _scheduler.CheckExists(jobKey)) return MsgHelper.Fail<bool>($"该任务计划已经在执行:【{sysSchedule.Name}】,请勿重复启动！");

                //通过反射获取程序集类型
                Type jobType = Assembly.Load(sysSchedule.AssemblyName).GetType($"{sysSchedule.NameSpace}.{sysSchedule.ClassName}");

                //如果任务调度没得开启就开启任务调度，只用启动一次
                if (!_scheduler.IsStarted) await StartScheduleAsync();

                //创建一个任务，提供执行任务的程序集class
                IJobDetail job = new JobDetailImpl(sysSchedule.Id.ToString(), sysSchedule.JobGroup, jobType);
                job.JobDataMap.Add("JobName", sysSchedule.Name);
                job.JobDataMap.Add("JobParam", sysSchedule.JobParams);//应该是加入参数

                //创建触发器（两种时间格式的触发器）
                ITrigger trigger = GetTrigger(sysSchedule);

                // 告诉Quartz使用我们的触发器来安排作业
                await _scheduler.ScheduleJob(job, trigger);

                return MsgHelper.Success(true, $"启动任务:【{ sysSchedule.Name}】成功");
            }
            catch (Exception ex)
            {
                return MsgHelper.Fail<bool>($"任务计划异常:【{ex.Message}】");
            }
        }

        /// <summary>
        /// 暂停一个指定的计划任务
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<bool>> StopJobAsync(TimedTask sysSchedule)
        {
            try
            {
                JobKey jobKey = new(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                if (!await _scheduler.CheckExists(jobKey)) return MsgHelper.Fail<bool>($"未找到要暂停的任务:【{sysSchedule.Name}】");
                else
                {
                    await _scheduler.DeleteJob(jobKey);
                    return MsgHelper.Success(true, $"暂停任务:【{sysSchedule.Name}】成功");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 重新安排任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> RescheduleJobAsync(TimedTask sysSchedule)
        {
            try
            {
                JobKey jobKey = new(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                if (!await _scheduler.CheckExists(jobKey)) return MsgHelper.Fail<bool>($"未找到要重新的任务:【{sysSchedule.Name}】,请先选择添加计划！");

                //创建触发器（两种时间格式的触发器）
                ITrigger trigger = GetTrigger(sysSchedule);

                //得到指定任务重启任务
                TriggerKey triggerKey = new(sysSchedule.Id.ToString(), sysSchedule.JobGroup);
                await _scheduler.RescheduleJob(triggerKey, trigger);

                return MsgHelper.Success(true, $"恢复计划任务:【{sysSchedule.Name}】成功");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion 管理任务

        #region 创建触发器帮助方法

        /// <summary>
        /// 获取 触发器
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        private static ITrigger GetTrigger(TimedTask sysSchedule)
        {
            TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(sysSchedule.Id.ToString(), sysSchedule.JobGroup)
                .StartAt(sysSchedule.BeginTime)//开始时间
                .EndAt(sysSchedule.EndTime)//结束数据
                .ForJob(sysSchedule.Id.ToString(), sysSchedule.JobGroup);//作业名称

            if (sysSchedule.Cron != null && CronExpression.IsValidExpression(sysSchedule.Cron) && sysSchedule.TriggerType == TriggerType.Cron)
                triggerBuilder.WithCronSchedule(sysSchedule.Cron);//指定cron表达式
            else
                triggerBuilder.WithSimpleSchedule(x => x.WithIntervalInSeconds(sysSchedule.IntervalSecond).RepeatForever());

            return triggerBuilder.Build();
        }

        #endregion 创建触发器帮助方法

        #region 开启和停止任务调度

        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<bool>> StartScheduleAsync()
        {
            try
            {
                _scheduler.JobFactory = _jobFactory;
                if (_scheduler.IsStarted) return MsgHelper.Fail<bool>("任务调度已经开启");
                else
                {
                    //等待任务运行完成
                    await _scheduler.Start();
                    return MsgHelper.Success(true, "任务调度开启成功");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<bool>> StopScheduleAsync()
        {
            try
            {
                if (_scheduler.IsShutdown) return MsgHelper.Fail<bool>("任务调度不是开始状态");
                else
                {
                    //等待任务运行完成
                    await _scheduler.Shutdown();
                    GetSchedulerAsync(true);
                    return MsgHelper.Success(true, "任务调度停止成功");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion 开启和停止任务调度
    }
}