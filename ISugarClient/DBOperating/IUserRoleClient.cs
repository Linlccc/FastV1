using Attributes.SqlSugar;
using Model.Models;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// UserRole 仓储接口
    /// </summary>
    public interface IUserRoleClient : IBaseClient<UserRole>
    {
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="roleIds">角色id集合</param>
        /// <returns>是否成功</returns>
        [Tran]
        Task<bool> SetUserRoles(long userId, long[] roleIds);
    }
}