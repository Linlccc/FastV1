using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 自定义路由 /api/{version}/[controler]/[action]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomRouteAttribute : ApiExplorerSettingsAttribute, IRouteTemplateProvider
    {
        public string Template { get; set; }

        public int? Order { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 自定义版本+路由构造函数，继承基类路由
        /// </summary>
        /// <param name="group">api组</param>
        /// <param name="actionName">方法名</param>
        /// <param name="ignoreApi">是否忽略不显示该api</param>
        public CustomRouteAttribute(ApiGroup group = ApiGroup.Default, string actionName = "[action]", bool ignoreApi = false)
        {
            if (group == ApiGroup.Default) Template = $"api/[controller]/{actionName}";
            else Template = $"api/{group}/[controller]/{actionName}";
            GroupName = group.ToString();//这个是用来区分显示在那个组
            IgnoreApi = ignoreApi;
        }
    }

    /// <summary>
    /// Api 版本
    /// </summary>
    public enum ApiGroup
    {
        Default,
        Consul,
        Test
    }
}