using Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Features 仓储类
    /// </summary>
    public class FeaturesClient : BaseClient<Feature>, IFeaturesClient
    {
        public FeaturesClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<Feature>> GetUserFeatures(long userId)
        {
            //查询该角色可以使用的所有功能
            List<Feature> features = await SugarClient.Queryable<UserRole, Role, RoleFeature, Feature>
                ((ur, r, rf, f) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id, JoinType.Left, ur.RoleId == rf.RoleId, JoinType.Left, rf.FeaturesId == f.Id))
                .Where((ur, r, rf, f) => ur.UserId == userId)
                .Where((ur, r, rf, f) => r.Enabled)
                .Where((ur, r, rf, f) => !string.IsNullOrWhiteSpace(f.Name) && f.Enabled)
                .Select((ur, r, rf, f) => f)
                .Distinct()
                .ToListAsync();
            return features;
        }

        public async Task<List<Feature>> GetUserFeatureTree(long userId) => ConfigurationRelationship(await GetUserFeatures(userId)).ChildFeaturesList;

        public async Task<List<Feature>> GetAllFeaturesTree()
        {
            List<Feature> features = await QueryAsync();
            return ConfigurationRelationship(features).ChildFeaturesList;
        }

        public async Task<bool> RecursiveDeleteById(long id)
        {
            List<Feature> features = await QueryAsync();
            List<long> deleteIds = new() { id };
            GetAllChildId(features, deleteIds, id);
            return await DeleteByIdAsync(deleteIds);
        }

        #region 帮助方法

        /// <summary>
        /// 获取所有子孙id
        /// </summary>
        /// <param name="features">功能列表</param>
        /// <param name="allChildIds">子孙id集合</param>
        /// <param name="id">父id</param>
        /// <returns></returns>
        private void GetAllChildId(List<Feature> features, List<long> allChildIds, long id)
        {
            List<long> childIds = features.Where(f => f.FatherId == id).Select(f => f.Id).ToList();
            allChildIds.AddRange(childIds);

            childIds.ForEach(c => GetAllChildId(features, allChildIds, c));
        }

        /// <summary>
        /// 配置指定功能的所有下级关系
        /// </summary>
        /// <param name="features">功能列表</param>
        /// <param name="currentF">需要配置的功能,如果从根开始可以不传</param>
        /// <returns>返回配置的功能，包含子功能</returns>
        private Feature ConfigurationRelationship(List<Feature> features, Feature currentF = null)
        {
            if (currentF == null)
            {
                currentF = new Feature()
                {
                    Id = -1,
                    Name = "根节点"
                };
            }
            long fId = currentF.Id == -1 ? 0 : currentF.Id;
            currentF.ChildFeaturesList = features.Where(f => f.FatherId == fId).OrderByDescending(f => f.Sort).ToList();

            foreach (Feature f in currentF.ChildFeaturesList)
            {
                ConfigurationRelationship(features, f);
            }
            return currentF;
        }

        #endregion 帮助方法
    }
}