using Quartz;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Tasks.Jobs
{
    /// <summary>
    /// 是一个很简单的任务类，反复获取用户个数
    /// </summary>
    public class GetUserCountTask : IJob
    {
        private readonly IUserClient _userClient;

        public GetUserCountTask(IUserClient userClient)
        {
            _userClient = userClient;
        }

        /// <summary>
        /// 执行任务的方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"当前用户数：{await _userClient.CountAsync()}");
        }
    }
}