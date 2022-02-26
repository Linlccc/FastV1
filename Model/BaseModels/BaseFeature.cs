using SqlSugar;

namespace Model.BaseModels
{
    /// <summary>
    /// 功能 父类
    /// </summary>
    public class BaseFeature : RootEntity
    {
        /// <summary>
        /// 功能名称（显示）
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100)]
        public string Name { get; set; }

        /// <summary>
        /// 页面Name
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string PageName { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 父级id 可空
        /// </summary>
        public long FatherId { get; set; }

        /// <summary>
        /// 是否是元素
        /// </summary>
        public bool IsElement { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string Icon { get; set; }

        /// <summary>
        /// 访问路由地址
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string Route { get; set; }

        /// <summary>
        /// 重定向地址
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Redirect { get; set; }

        /// <summary>
        /// 访问路由地址
        /// 示例 views/Home.vue
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string ViewPath { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 隐藏（不能搜索，不显示在侧边栏）
        /// 页面使用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool Hidden { get; set; }

        /// <summary>
        /// 显示在ViewTags
        /// </summary>
        public bool ShowVisited { get; set; }

        /// <summary>
        /// 是否固定视图
        /// 要显示固定视图必须启动ShowVisited
        /// </summary>
        public bool AffixView { get; set; }

        /// <summary>
        /// 缓存视图
        /// </summary>
        public bool CacheView { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}