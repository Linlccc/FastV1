using Attributes.Entity;
using Model.BaseModels;
using SqlSugar;

namespace Model.Models
{
    [Entity]
    public class RoleInterface : BaseRoleInterface
    {
        #region 导航属性

        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Role Role { get; set; }

        /// <summary>
        /// 接口
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public InterfaceInfo InterfaceInfo { get; set; }

        #endregion 导航属性
    }
}