using System.Diagnostics;

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// 机器工具
    /// </summary>
    public class MachineUtil
    {
        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        public static string GetCPURate()
        {
            string cpuRate;
            if (IsUnix())
            {
                string output = ShellUtil.Bash("top -b -n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
                cpuRate = output.Trim();
            }
            else
            {
                string output = ShellUtil.Cmd("wmic", "cpu get LoadPercentage");
                cpuRate = output.Replace("LoadPercentage", string.Empty).Trim();
            }
            return cpuRate;
        }

        /// <summary>
        /// 是否Linux
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}