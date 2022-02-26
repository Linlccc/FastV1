namespace System.Diagnostics
{
    /// <summary>
    /// 系统Shell命令
    /// </summary>
    public class ShellUtil
    {
        /// <summary>
        /// Bash命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string Bash(string command)
        {
            string escapedArgs = command.Replace("\"", "\\\"");
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();
            return result;
        }

        /// <summary>
        /// cmd命令
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Cmd(string fileName, string args)
        {
            string output = string.Empty;
            ProcessStartInfo info = new()
            {
                FileName = fileName,
                Arguments = args,
                RedirectStandardOutput = true
            };
            using (Process process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }
            return output;
        }
    }
}