using Attributes.Tool;
using static Attributes.Tool.RemarkExtension;

namespace System
{
    /// <summary>
    /// 执行Sql日志
    /// </summary>
    public class SqlLogInfo
    {
        public SqlLogInfo()
        { }

        public SqlLogInfo(string parameters, string sql)
        {
            Parameters = parameters;
            Sql = sql.Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// 执行时间
        /// </summary>
        [Remark("【执行时间】：")]
        public DateTime ExecuteTime { get; set; } = DateTime.Now;

        /// <summary>
        /// sql参数
        /// </summary>
        [Remark("【Sql参数】：")]
        public string Parameters { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        [Remark("【Sql语句】：")]
        public string Sql { get; set; }

        /// <summary>
        /// 工作时间ms
        /// </summary>
        [Remark("【工作时间ms】：")]
        public string WorkTime { get; set; } = "N/A";

        public override string ToString()
        {
            return
                GetMemberRemark<SqlLogInfo>(nameof(ExecuteTime)) + ExecuteTime + Environment.NewLine +
                GetMemberRemark<SqlLogInfo>(nameof(WorkTime)) + WorkTime + Environment.NewLine +
                GetMemberRemark<SqlLogInfo>(nameof(Parameters)) + Parameters + Environment.NewLine +
                GetMemberRemark<SqlLogInfo>(nameof(Sql)) + Sql + Environment.NewLine;
        }
    }
}