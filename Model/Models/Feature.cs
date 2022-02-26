using Attributes.Entity;
using Model.BaseModels;
using SqlSugar;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Model.Models
{
    /// <summary>
    /// 功能
    /// </summary>
    [Entity]
    public class Feature : BaseFeature
    {
        #region 导航元素

        /// <summary>
        /// 下面的子功能
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [JsonIgnore]
        public List<Feature> ChildFeaturesList { get; set; }

        #endregion 导航元素
    }
}