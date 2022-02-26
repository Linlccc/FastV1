using Model.Models;
using System;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// UserInfo 仓储接口
    /// </summary>
    public interface IUserClient : IBaseClient<User>
    {
        /// <summary>
        /// 根据id获取 用户 及 用户的角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns>用户信息，包含角色</returns>
        Task<User> QueryUserRolesById(long id);

        /// <summary>
        /// 分页获取用户列表
        /// </summary>
        /// <param name="name">搜索的用户名称</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        Task<PageMsg<User>> PageList(string name, int pageIndex, int pageSize);
    }
}