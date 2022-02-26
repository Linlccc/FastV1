using Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Features 仓储接口
    /// </summary>
    public interface IFeaturesClient : IBaseClient<Feature>
    {
        /// <summary>
        /// 获取用户的功能信息列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Feature>> GetUserFeatures(long userId);

        /// <summary>
        /// 获取用户的功能信息树
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Feature>> GetUserFeatureTree(long userId);

        /// <summary>
        /// 获取全部功能信息树
        /// </summary>
        /// <returns></returns>
        Task<List<Feature>> GetAllFeaturesTree();

        /// <summary>
        /// 删除功能，递归删除所有子孙功能
        /// </summary>
        /// <param name="id">要删除的功能id</param>
        /// <returns></returns>
        Task<bool> RecursiveDeleteById(long id);
    }
}