using Attributes.Tool;
using static Attributes.Tool.RemarkExtension;

namespace System
{
    /// <summary>
    /// 数据操作日志
    /// 文档
    /// </summary>
    public class DbOperLogInfo
    {
        /// <summary>
        /// 操作时间
        /// </summary>
        [Remark("【操作时间】：")]
        public DateTime OperTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 操作用户
        /// </summary>
        [Remark("【操作用户】：")]
        public string OperUserName { get; set; }

        /// <summary>
        /// 操作用户id
        /// </summary>
        [Remark("【操作用户Id】：")]
        public string OperUserId { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Remark("【操作类型】：")]
        public string OperType { get; set; }

        /// <summary>
        /// 操作的方法
        /// </summary>
        [Remark("【操作方法】：")]
        public string OperMethod { get; set; }

        /// <summary>
        /// 携带的参数
        /// </summary>
        [Remark("【携带参数】：")]
        public string Parameters { get; set; }

        /// <summary>
        /// 工作时间ms
        /// </summary>
        [Remark("【工作时间ms】：")]
        public string WorkTime { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        [Remark("【结果】：")]
        public string Result { get; set; }

        public override string ToString()
        {
            return
                GetMemberRemark<DbOperLogInfo>(nameof(OperTime)) + OperTime + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(OperUserName)) + OperUserName + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(OperUserId)) + OperUserId + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(OperType)) + OperType + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(OperMethod)) + OperMethod + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(Parameters)) + Parameters + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(WorkTime)) + WorkTime + Environment.NewLine +
                GetMemberRemark<DbOperLogInfo>(nameof(Result)) + Result + Environment.NewLine;
        }
    }
}