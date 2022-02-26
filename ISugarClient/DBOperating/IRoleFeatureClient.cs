using Attributes.SqlSugar;
using Model.Models;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// RoleFeatures 仓储接口
    /// </summary>
    public interface IRoleFeatureClient : IBaseClient<RoleFeature>
    {
        /// <summary>
        /// 设置角色权限（功能）
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="featureIds">功能id集合</param>
        /// <returns></returns>
        [Tran]
        Task<bool> SetRoleFeatures(long roleId, long[] featureIds);
    }
}