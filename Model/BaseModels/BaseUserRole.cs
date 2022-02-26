using SqlSugar;

namespace Model.BaseModels
{
    /// <summary>
    /// 用户和角色 连接 父类
    /// </summary>
    public class BaseUserRole : RootEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long UserId { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long RoleId { get; set; }
    }
}