using Model.Models;

namespace SqlSugar
{
    /// <summary>
    /// Role 仓储类
    /// </summary>
    public class RoleClient : BaseClient<Role>, IRoleClient
    {
        public RoleClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}