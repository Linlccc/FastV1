using System.Collections.Generic;

namespace Model.ViewModels
{
    public class AppInfo
    {
        /// <summary>
        /// 是否是64位进程
        /// </summary>
        public bool Is64BitProcess { get; set; }

        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// 程序根目录
        /// </summary>
        public string AppRootPath { get; set; }

        /// <summary>
        /// WebRootPath
        /// </summary>
        public string WebRootPath { get; set; }

        /// <summary>
        /// .NET Core 框架描述
        /// </summary>
        public string FrameworkDescription { get; set; }

        /// <summary>
        /// 内存占用
        /// </summary>
        public string WorkingSet { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public string WorkingTime { get; set; }

        /// <summary>
        /// 占用cpu时间
        /// </summary>
        public string UseCpuTime { get; set; }
    }

    /// <summary>
    /// 系统信息
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// 操作系统及版本
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// 是否是64位操作系统
        /// </summary>
        public bool Is64BitOperatingSystem { get; set; }

        /// <summary>
        /// 机器名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 与当前用户关联的网络域名
        /// </summary>
        public string UserDomainName { get; set; }

        /// <summary>
        /// 逻辑分区
        /// </summary>
        public string[] LogicalDrives { get; set; }

        /// <summary>
        /// 系统目录
        /// </summary>
        public string SystemDirectory { get; set; }

        /// <summary>
        /// 系统已运行时间s
        /// </summary>
        public int RunTimes { get; set; }

        /// <summary>
        /// 是否在交互模式中运行
        /// </summary>
        public bool UserInteractive { get; set; }

        /// <summary>
        /// 当前关联的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 公共语言运行时的版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
    }
}