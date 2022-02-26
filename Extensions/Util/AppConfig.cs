using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// 项目配置
    /// </summary>
    public static class AppConfig
    {
        #region 配置
        private static IConfiguration _configuration;

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="builder"></param>
        public static void AddConfigureFiles(HostBuilderContext hostBuilder, IConfigurationBuilder builder)
        {
            IConfigurationRoot Configuration = builder.Build();
            //得到要忽略的配置文件
            string[] ignoreConfigurationFiles = Configuration.GetNode<string[]>("ConfigInfo", "IgnoreConfigurationFiles") ?? Array.Empty<string>();
            //得到自定义配置文件夹
            string customConfigFolder = Configuration.GetNode("ConfigInfo", "CustomFolder");

            //得到自定义配置文件
            IEnumerable<string> jsonFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, customConfigFolder), "*.json", SearchOption.TopDirectoryOnly)
                .Union(Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), customConfigFolder), "*.json", SearchOption.TopDirectoryOnly))
                .Where(u => CheckIncludeDefaultSettings(Path.GetFileName(u)) && !ignoreConfigurationFiles.Contains(Path.GetFileName(u)) && !_runtimeJsonSuffixs.Any(j => u.ToLower().EndsWith(j)));

            //生产环境Json
            IEnumerable<string> proJsonFiles = jsonFiles.Where(j => !_developmentJsonSuffixs.Any(d => j.ToLower().EndsWith(d)));
            //添加生产自定义配置文件
            foreach (string file in proJsonFiles) builder.AddJsonFile(file, optional: true, reloadOnChange: true);

            //添加开发自定义配置文件
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "development")
            if (hostBuilder.HostingEnvironment.IsDevelopment())
            {
                IEnumerable<string> devJsonFiles = jsonFiles.Where(j => _developmentJsonSuffixs.Any(d => j.ToLower().EndsWith(d)));
                foreach (string file in devJsonFiles) builder.AddJsonFile(file, optional: true, reloadOnChange: true);//添加开发自定义配置文件
            }

            _configuration = builder.Build();
        }

        /// <summary>
        /// 排除运行时 Json 后缀
        /// </summary>
        private static readonly string[] _runtimeJsonSuffixs = new[]
        {
            "deps.json",
            "runtimeconfig.dev.json",
            "runtimeconfig.prod.json",
            "runtimeconfig.json"
        };

        /// <summary>
        /// 测试环境json后缀
        /// </summary>
        private static readonly string[] _developmentJsonSuffixs =
        {
            ".development.json"
        };

        /// <summary>
        /// 排除特定配置文件正则表达式
        /// </summary>
        private const string _excludeJsonPattern = @"^{0}(\.\w+)?\.((json)|(xml))$";

        /// <summary>
        /// 排除的配置文件前缀
        /// </summary>
        private static readonly string[] _excludeJsonPrefixs = new[] { "appsettings", "bundleconfig", "compilerconfig" };

        /// <summary>
        /// 检查是否是默认排除的配置文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool CheckIncludeDefaultSettings(string fileName)
        {
            foreach (string prefix in _excludeJsonPrefixs)
            {
                if (fileName.IsMatch(string.Format(_excludeJsonPattern, prefix))) return false;
            }

            return true;
        }

        #endregion 配置

        #region 获取配置信息

        /// <summary>
        /// 获取指定节点下的信息
        /// </summary>
        /// <param name="configuration">配置对象</param>
        /// <param name="sections">节点</param>
        /// <returns>节点下的信息，如果没有则返回 null</returns>
        public static string GetNode(this IConfiguration configuration, params string[] sections) => configuration[string.Join(":", sections)];

        public static string GetNode(params string[] sections) => _configuration.GetNode(sections);

        /// 获取bool类型的节点数据
        /// </summary>
        /// <param name="configuration">配置对象</param>
        /// <param name="sections">节点</param>
        /// <returns>节点数据时true时返回true，其他都返回false</returns>
        public static bool GetBoolNode(this IConfiguration configuration, params string[] sections) => configuration.GetNode(sections).OToBool();

        public static bool GetBoolNode(params string[] sections) => _configuration.GetBoolNode(sections);

        /// <summary>
        /// 获取指定节点下的数据
        /// </summary>
        /// <typeparam name="T">要获取的数据类型</typeparam>
        /// <param name="configuration">配置对象</param>
        /// <param name="sections">节点</param>
        /// <returns>返回指定类型的数据</returns>
        public static T GetNode<T>(this IConfiguration configuration, params string[] sections) => configuration.GetSection(string.Join(":", sections)).Get<T>();

        public static T GetNode<T>(params string[] sections) => _configuration.GetNode<T>(sections);

        #endregion 获取配置信息

        #region 服务

        /// <summary>
        /// 程序根服务
        /// </summary>
        public static IServiceProvider RootServices { get; private set; }

        /// <summary>
        /// 配置根服务
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHost InitService(this IHost host)
        {
            RootServices = host.Services;
            return host;
        }

        #endregion 服务
    }
}