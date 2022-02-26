using Attributes.Entity;
using Model.BaseModels;
using SqlSugar;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model.Models
{
    /// <summary>
    /// 用户类
    /// </summary>
    [Entity]
    public class User : BaseUser
    {
        /// <summary>
        /// 明文密码
        /// </summary>
        public string PlaintextPwd { get; set; }

        /// <summary>
        /// 错误次数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int ErrorCount { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 2000, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// 后台的 signalr 连接
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string AdminSignalrConnectionId { get; set; }

        #region 导航属性

        /// <summary>
        /// 角色信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]//数据库忽略
        [JsonIgnore]
        public List<Role> Roles { get; set; }

        #endregion 导航属性
    }
}