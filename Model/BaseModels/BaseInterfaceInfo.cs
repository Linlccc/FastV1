using SqlSugar;
using System;

namespace Model.BaseModels
{
    /// <summary>
    /// 接口信息父类
    /// </summary>
    public class BaseInterfaceInfo : RootEntity
    {
        /// <summary>
        /// 接口链接
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100)]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 接口功能描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }
    }
}