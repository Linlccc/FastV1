using Attributes.SqlSugar;
using Model.Models;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Role interface 仓储接口
    /// </summary>
    public interface IRoleInterfaceClient : IBaseClient<RoleInterface>
    {
        /// <summary>
        /// 设置角色的接口
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        [Tran]
        Task<bool> SetRoleInterfaces(long roleId, long[] interfaces);
    }
}