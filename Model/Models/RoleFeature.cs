using Attributes.Entity;
using Model.BaseModels;
using SqlSugar;

namespace Model.Models
{
    /// <summary>
    /// 角色和功能关系
    /// </summary>
    [Entity]
    public class RoleFeature : BaseRoleFeature
    {
        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Role Role { get; set; }

        /// <summary>
        /// 功能
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Feature Features { get; set; }
    }
}