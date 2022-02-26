using SqlSugar;
using System;

namespace Model.BaseModels
{
    /// <summary>
    /// 用户父类
    /// </summary>
    public class BaseUser : RootEntity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string NickName { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = false)]
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = false)]
        public string LoginPWD { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Avatar { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        ///最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; } = DateTime.Now;
    }
}