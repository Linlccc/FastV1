namespace FastTool.GlobalVar
{
    /// <summary>
    /// 请求日志信息
    /// </summary>
    public static class LogFolderInfo
    {
        /// <summary>
        /// 访问 日志文件夹
        /// 这个是直接看的
        /// </summary>
        public const string AccessFolder = "AccessLog";

        /// <summary>
        /// 数据库操作 日志文件夹
        /// </summary>
        public const string DbOperFolder = "DbOperLog";

        /// <summary>
        /// 执行sql 日志文件夹
        /// </summary>
        public const string SqlLogFolder = "SqlLog";

        /// <summary>
        /// 执行sql错误 日志文件夹
        /// </summary>
        public const string ErrorSqlLogFolder = "ErrorSqlLog";
    }
}