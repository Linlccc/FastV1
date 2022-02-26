using Attributes.Tool;
using FastTool.GlobalVar;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace System
{
    public class LogHelper
    {
        public static string ContentRoot { get; set; } = string.Empty;  //存放日志文件的根路径

        //存放日志文件的路径
        private static string _logFolderPath;

        private static string LogFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_logFolderPath)) _logFolderPath = Path.Combine(ContentRoot, "Log");
                return _logFolderPath;
            }
        }

        //管理资源访问的锁定
        private static readonly ReaderWriterLockSlim _logWriteLock = new();

        //日志分割符
        private static readonly string _logDivider = "--------------------------------";

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="dataParas">内容</param>
        public static void WritrLog(string logType, params string[] dataParas)
        {
            try
            {
                _logWriteLock.EnterWriteLock();//进入写入模式，占用资源

                //得到可用的日志文件,如果该文件文件夹不存在创建
                string logFilepath = FileHelper.GetLogFileFullName(Path.Combine(LogFolderPath, logType));//获取可用的log记录文件
                FileInfo logFile = new(logFilepath);
                if (!logFile.Directory.Exists) Directory.CreateDirectory(logFile.Directory.FullName);

                //拼接日志
                string logContent = _logDivider + Environment.NewLine + string.Join(Environment.NewLine, dataParas) + Environment.NewLine;

                File.AppendAllText(logFilepath, logContent);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                //退出写入模式，释放资源占用
                _logWriteLock.ExitWriteLock();
            }
        }

        #region 读取日志

        /// <summary>
        /// 获取指定时间段的指定日志
        /// </summary>
        /// <param name="startTime">开始的时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="count">要取得日志条数</param>
        /// <returns></returns>
        public static List<T> GetLogInfos<T>(DateTime startTime = default, DateTime endTime = default, int count = 50) where T : new()
        {
            //获取要读取那个日志文件
            string logFolderName = string.Empty;
            if (typeof(T) == typeof(AccessLogInfo)) logFolderName = LogFolderInfo.AccessFolder;
            else if (typeof(T) == typeof(DbOperLogInfo)) logFolderName = LogFolderInfo.DbOperFolder;
            else if (typeof(T) == typeof(SqlLogInfo)) logFolderName = LogFolderInfo.SqlLogFolder;
            else if (typeof(T) == typeof(ErrorSqlLogInfo)) logFolderName = LogFolderInfo.ErrorSqlLogFolder;
            //读取日志文件
            string LogStr = ReadTimeSlotFiles(Path.Combine(LogFolderPath, logFolderName), startTime, endTime);
            List<List<string>> logs = LogStr.Split(_logDivider)
                .Reverse()
                .Where(l => l.IsNNull())
                .Select(l => l.Split(Environment.NewLine).Where(o => o.IsNNull()).ToList())
                .Take(count)
                .ToList();
            //日志转对象
            List<T> result = GetLogs<T>(logs);
            return result;
        }

        #endregion 读取日志

        #region 帮助

        /// <summary>
        /// 读取指定时间段的日志
        /// 默认读取今天的
        /// </summary>
        /// <param name="folderPath">要读取的问件路径</param>
        /// <param name="readStartDay">开始时间</param>
        /// <param name="readEndDay">结束时间</param>
        /// <returns></returns>
        private static string ReadTimeSlotFiles(string folderPath, DateTime readStartDay, DateTime readEndDay)
        {
            if (!Directory.Exists(folderPath)) return "";

            if (readStartDay == default) readStartDay = DateTime.Now.Date;
            if (readEndDay == default) readEndDay = DateTime.Now.Date.AddDays(1);

            _logWriteLock.EnterReadLock();
            StringBuilder stringBuilder = new();
            //得到今天的日志文件
            List<string> fileInfos = new DirectoryInfo(folderPath).GetFiles()
                .Where(f => DateTime.Parse(f.Name.Replace(f.Extension, "").Replace(".", ":")).IsInTimeSlot(readStartDay, readEndDay))
                .OrderBy(f => f.CreationTime)
                .Select(f => f.FullName)
                .ToList();
            //读取所有文件的内容
            fileInfos.ForEach(f =>
            {
                if (File.Exists(f))
                {
                    using StreamReader reader = new(f);
                    stringBuilder.Append(reader.ReadToEnd());
                }
            });

            _logWriteLock.ExitReadLock();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将日志集合转成日志对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessLogs"></param>
        /// <returns></returns>
        private static List<T> GetLogs<T>(List<List<string>> accessLogs) where T : new()
        {
            List<T> results = new();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();//得到对象的属性

            accessLogs.ForEach(log =>
            {
                //通过反射给对象赋值
                T t = new();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    string remark = propertyInfo.GetMemberRemark();
                    string text = log.Where(l => l.Contains(remark)).FirstOrDefault()?.Replace(remark, "");
                    object value = Convert.ChangeType(text, propertyInfo.PropertyType);
                    if (value.IsNNull()) propertyInfo.SetValue(t, value);
                }
                results.Add(t);
            });
            return results;
        }

        #endregion 帮助
    }
}