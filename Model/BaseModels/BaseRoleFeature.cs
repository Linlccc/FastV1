namespace Model.BaseModels
{
    /// <summary>
    /// 角色和功能的关系
    /// </summary>
    public class BaseRoleFeature : RootEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 功能ID
        /// </summary>
        public long FeaturesId { get; set; }
    }
}