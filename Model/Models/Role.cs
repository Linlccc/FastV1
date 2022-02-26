using Attributes.Entity;
using Attributes.SqlSugar;
using Model.BaseModels;
using SqlSugar;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model.Models
{
    /// <summary>
    /// 角色类
    /// </summary>
    [Entity, Connect("MyCore_MSSQL_b1")]
    public class Role : BaseRole
    {
        #region 导航属性

        /// <summary>
        /// 角色的功能表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [JsonIgnore]
        public List<Feature> FeaturesList { get; set; }

        #endregion 导航属性
    }
}