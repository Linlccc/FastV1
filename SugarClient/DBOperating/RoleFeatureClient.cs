using Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// RoleFeatures 仓储类
    /// </summary>
    public class RoleFeatureClient : BaseClient<RoleFeature>, IRoleFeatureClient
    {
        public RoleFeatureClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<bool> SetRoleFeatures(long roleId, long[] featureIds)
        {
            await DeleteAsync(rf => rf.RoleId == roleId);
            List<RoleFeature> roleFeatures = featureIds.ToList().Select(f => new RoleFeature() { RoleId = roleId, FeaturesId = f }).ToList();
            await InsertRangeAsync(roleFeatures);
            return true;
        }
    }
}