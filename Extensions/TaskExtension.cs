using System.Diagnostics;

namespace System.Threading.Tasks
{
    /// <summary>
    /// 任务 拓展
    /// </summary>
    public static class TaskExtension
    {
        #region 执行任务

        /// <summary>
        /// 获取任务执行时间
        /// [建议不要检测已经开始的任务，检测出来的数据不一定准确]
        /// </summary>
        /// <param name="task"></param>
        /// <returns>任务执行时间,如果执行出错 Exception 不为空</returns>
        public static async Task<Tuple<Stopwatch, Exception>> GetWorkTimeAsync(this Task task)
        {
            try
            {
                Stopwatch sw = new();
                sw.Start();
                if (task.Status == TaskStatus.Created) task.Start();
                await task;
                sw.Stop();
                return new Tuple<Stopwatch, Exception>(sw, null);
            }
            catch (Exception ex)
            {
                return new Tuple<Stopwatch, Exception>(null, ex);
            }
        }

        /// <summary>
        /// 获取任务执行时间
        /// [建议不要检测已经开始的任务，检测出来的数据不一定准确]
        /// </summary>
        /// <param name="task"></param>
        /// <returns>任务执行时间和任务的返回值,如果执行出错 Exception 不为空</returns>
        public static async Task<Tuple<Stopwatch, T, Exception>> GetWorkTimeAsync<T>(this Task<T> task)
        {
            try
            {
                Stopwatch sw = new();
                sw.Restart();
                if (task.Status == TaskStatus.Created) task.Start();
                T taskResult = await task;
                sw.Stop();
                return new Tuple<Stopwatch, T, Exception>(sw, taskResult, null);
            }
            catch (Exception ex)
            {
                return new Tuple<Stopwatch, T, Exception>(null, default, ex);
            }
        }

        #endregion 执行任务
    }
}