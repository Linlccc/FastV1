using static Microsoft.Extensions.Configuration.AppConfig;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// Aop配置信息
    /// </summary>
    public static class AopInfo
    {
        /// <summary>
        /// Redis缓存Aop
        /// </summary>
        public static bool RedisCachingAOP { get; } = GetBoolNode("AopSettings", "RedisCachingAOP");

        /// <summary>
        /// 本地缓存Aop
        /// </summary>
        public static bool MemoryCachingAOP { get; } = GetBoolNode("AopSettings", "MemoryCachingAOP");

        /// <summary>
        /// 数据操作Aop
        /// </summary>
        public static bool DbOperAOP { get; } = GetBoolNode("AopSettings", "DbOperAOP");

        /// <summary>
        /// 数据库事务Aop
        /// </summary>
        public static bool TranAOP { get; } = GetBoolNode("AopSettings", "TranAOP");

        /// <summary>
        /// Sql 执行Aop
        /// </summary>
        public static bool SqlAOP { get; } = GetBoolNode("AopSettings", "SqlAOP");
    }
}