using Attributes.Entity;
using Model.BaseModels;
using SqlSugar;

namespace Model.Models
{
    /// <summary>
    /// 用户和角色关联类
    /// </summary>
    [Entity]
    public class UserRole : BaseUserRole
    {
        //数据库忽略

        /// <summary>
        /// 用户
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public User User { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Role Role { get; set; }
    }
}