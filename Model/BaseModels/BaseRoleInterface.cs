namespace Model.BaseModels
{
    /// <summary>
    /// 角色和接口的关系
    /// </summary>
    public class BaseRoleInterface : RootEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 接口ID
        /// </summary>
        public long InterfaceId { get; set; }
    }
}