using static Microsoft.Extensions.Configuration.AppConfig;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// 中间件配置变量
    /// </summary>
    public static class MiddlewareInfo
    {
        /// <summary>
        /// 访问纪录
        /// </summary>
        public static bool UserAccessLog { get; } = GetBoolNode("Middleware", "AccessLog", "Enabled");

        /// <summary>
        /// 实时发送日志
        /// </summary>
        public static bool SignalrRealTimeLog { get; } = GetBoolNode("Middleware", "SignalrRealTimeLog", "Enabled");

        /// <summary>
        /// IP限流
        /// </summary>
        public static bool IpRateLimit { get; } = GetBoolNode("Middleware", "IpRateLimit", "Enabled");

        /// <summary>
        /// 测试用户
        /// </summary>
        public static bool TestAuthUser { get; } = GetBoolNode("Middleware", "TestAuthUser", "Enabled");

        /// <summary>
        /// 性能检测
        /// </summary>
        public static bool MiniProfiler { get; } = GetBoolNode("Middleware", "MiniProfiler", "Enabled");
    }
}