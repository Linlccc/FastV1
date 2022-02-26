using Model.Models;
using System;
using System.Threading.Tasks;

namespace Tasks.Quartz
{
    /// <summary>
    /// 服务调度接口
    /// </summary>
    public interface ISchedulerCenterServer
    {
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<Msg<bool>> StartScheduleAsync();

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<Msg<bool>> StopScheduleAsync();

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Msg<bool>> StartJobAsync(TimedTask sysSchedule);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Msg<bool>> StopJobAsync(TimedTask sysSchedule);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Msg<bool>> RescheduleJobAsync(TimedTask sysSchedule);
    }
}