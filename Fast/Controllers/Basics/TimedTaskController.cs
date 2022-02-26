using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Tasks.Quartz;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [ApiController, CustomRoute]
    public class TimedTaskController : Controller
    {
        private readonly ILogger<TimedTaskController> _logger;
        private readonly ITimedTaskClient _timedTaskClient;
        private readonly ISchedulerCenterServer _schedulerCenterServer;

        public TimedTaskController(ILogger<TimedTaskController> logger, ITimedTaskClient timedTaskClient, ISchedulerCenterServer schedulerCenterServer)
        {
            _logger = logger;
            _timedTaskClient = timedTaskClient;
            _schedulerCenterServer = schedulerCenterServer;
        }

        #region 查询

        /// <summary>
        /// 分页获取任务信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Msg<PageMsg<TimedTask>>> GetPageList(string name, int pageIndex = 1, int pageSize = 10)
        {
            return MsgHelper.Success(await _timedTaskClient.QueryPageAsync(t => t.Name.Contains(name), pageIndex, pageSize));
        }

        #endregion 查询

        #region 操作

        /// <summary>
        /// 操作任务
        /// </summary>
        /// <param name="timedTask"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Oper(TimedTask timedTask)
        {
            if (await _timedTaskClient.StorageableAsync(timedTask)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 操作

        #region 删除

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Delete(long id)
        {
            TimedTask timedTask = await _timedTaskClient.QueryByIdAsync(id);
            if (timedTask == null) return MsgHelper.Fail("没有找到任务，请刷新后重试", false);

            if (timedTask.IsStart && !(await _schedulerCenterServer.StopJobAsync(timedTask)).Success) return MsgHelper.Fail("停止任务失败，请刷新重试", false);
            if (await _timedTaskClient.DeleteAsync(timedTask)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 删除

        #region 操作任务

        /// <summary>
        /// 开始一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Start(long id)
        {
            TimedTask task = await _timedTaskClient.QueryByIdAsync(id);
            if (task == null) return MsgHelper.Fail<bool>("没有指定任务");

            return await _schedulerCenterServer.StartJobAsync(task);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Stop(long id)
        {
            TimedTask task = await _timedTaskClient.QueryByIdAsync(id);
            if (task == null) return MsgHelper.Fail<bool>("没有指定任务");

            return await _schedulerCenterServer.StopJobAsync(task);
        }

        /// <summary>
        /// 重新安排任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Reschedule(long id)
        {
            TimedTask task = await _timedTaskClient.QueryByIdAsync(id);
            if (task == null) return MsgHelper.Fail<bool>("没有指定任务");

            return await _schedulerCenterServer.RescheduleJobAsync(task);
        }

        /// <summary>
        /// 开始任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<bool>> StartSchedule() => await _schedulerCenterServer.StartScheduleAsync();

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<bool>> StopSchedule() => await _schedulerCenterServer.StopScheduleAsync();

        #endregion 操作任务
    }
}