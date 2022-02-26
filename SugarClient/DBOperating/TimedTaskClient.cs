using Model.Models;

namespace SqlSugar
{
    /// <summary>
    /// TimedTasks 仓储类
    /// </summary>
    public class TimedTaskClient : BaseClient<TimedTask>, ITimedTaskClient
    {
        public TimedTaskClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}