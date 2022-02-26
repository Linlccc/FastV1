using System.Collections.Generic;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// api 请求方法 配置
    /// </summary>
    public class ApiAction
    {
        /// <summary>
        /// 是否保留完全方法名
        /// </summary>
        public const bool IsKeepFullApiName = false;

        /// <summary>
        /// 请求方法字典
        /// </summary>
        public const string DefaultHttpMethod = "POST";

        /// <summary>
        /// 请求方法字典
        /// </summary>
        public static Dictionary<string, string> VerbToHttpMethods = new()
        {
            ["post"] = "POST",
            ["add"] = "POST",
            ["create"] = "POST",
            ["insert"] = "POST",
            ["submit"] = "POST",

            ["get"] = "GET",
            ["find"] = "GET",
            ["fetch"] = "GET",
            ["query"] = "GET",
            ["getlist"] = "GET",
            ["getall"] = "GET",

            ["put"] = "PUT",
            ["update"] = "PUT",

            ["delete"] = "DELETE",
            ["remove"] = "DELETE",
            ["clear"] = "DELETE",

            ["patch"] = "PATCH"
        };
    }
}