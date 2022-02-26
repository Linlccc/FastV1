using SqlSugar;
using System;

namespace Model.BaseModels
{
    /// <summary>
    /// 角色父类
    /// </summary>
    public class BaseRole : RootEntity
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50)]
        public string Name { get; set; }

        /// <summary>
        ///描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}