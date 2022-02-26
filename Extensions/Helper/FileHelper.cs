using System.Linq;

namespace System.IO
{
    /// <summary>
    /// 文件帮助
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfix(string fileName) => fileName[fileName.LastIndexOf(".")..^0];

        /// <summary>
        /// 指定文件夹中符合要求的日志文件,只限今天
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="size">日志文件最大容量</param>
        /// <returns></returns>
        public static string GetLogFileFullName(string folderPath, int size = 1 * 1024 * 1024)
        {
            if (Directory.Exists(folderPath))
            {
                FileInfo fileInfo = new DirectoryInfo(folderPath).GetFiles()
                .Where(f => f.Length < size && f.CreationTime.IsSameDay())
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();
                if (fileInfo != null) return fileInfo.FullName;
            }
            return Path.Combine(folderPath, $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.log");
        }
    }
}