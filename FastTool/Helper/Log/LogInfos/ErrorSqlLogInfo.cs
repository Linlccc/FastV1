using Attributes.Tool;
using static Attributes.Tool.RemarkExtension;

namespace System
{
    /// <summary>
    /// 执行Sql日志
    /// </summary>
    public class ErrorSqlLogInfo
    {
        public ErrorSqlLogInfo()
        { }

        public ErrorSqlLogInfo(string parameters, string sql, string stackTrace)
        {
            Parameters = parameters;
            Sql = sql.Replace("\r", "").Replace("\n", "");
            StackTrace = stackTrace;
        }

        /// <summary>
        /// 报错时间
        /// </summary>
        [Remark("【报错时间】：")]
        public DateTime ErrorTime { get; set; } = DateTime.Now;

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
        [Remark("【堆栈跟踪】：")]
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return
                GetMemberRemark<ErrorSqlLogInfo>(nameof(ErrorTime)) + ErrorTime + Environment.NewLine +
                GetMemberRemark<ErrorSqlLogInfo>(nameof(Parameters)) + Parameters + Environment.NewLine +
                GetMemberRemark<ErrorSqlLogInfo>(nameof(Sql)) + Sql + Environment.NewLine +
                GetMemberRemark<ErrorSqlLogInfo>(nameof(StackTrace)) + StackTrace + Environment.NewLine;
        }
    }
}