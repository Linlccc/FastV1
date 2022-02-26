using Attributes.Tool;
using static Attributes.Tool.RemarkExtension;

namespace System
{
    /// <summary>
    /// 用户访问日志
    /// </summary>
    public class AccessLogInfo
    {
        public AccessLogInfo()
        { }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Remark("【用户名称】：")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Remark("【用户Id】：")]
        public string UserId { get; set; }

        /// <summary>
        /// 用户ip
        /// </summary>
        [Remark("【用户Ip】：")]
        public string Ip { get; set; }

        /// <summary>
        /// 客户端信息
        /// </summary>
        [Remark("【用户端信息】：")]
        public string Agent { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        [Remark("【请求时间】：")]
        public DateTime RequestTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 本次请求的星期
        /// </summary>
        [Remark("【请求星期】：")]
        public string Week { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [Remark("【请求方法】：")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        [Remark("【请求路径】：")]
        public string RequestPath { get; set; }

        /// <summary>
        /// url 查询内容
        /// </summary>
        [Remark("【请求查询字段】：")]
        public string RequestQuery { get; set; }

        /// <summary>
        /// 请求Body
        /// </summary>
        [Remark("【请求Body】：")]
        public string RequestBody { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        [Remark("【响应时间】：")]
        public DateTime ResponseTime { get; set; }

        /// <summary>
        /// 工作时间ms
        /// </summary>
        [Remark("【工作时间ms】：")]
        public string WorkTime { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        [Remark("【响应数据】：")]
        public string ResponseData { get; set; }

        public override string ToString()
        {
            return
                GetMemberRemark<AccessLogInfo>(nameof(UserName)) + UserName + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(UserId)) + UserId + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(Ip)) + Ip + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(Agent)) + Agent + Environment.NewLine +

                GetMemberRemark<AccessLogInfo>(nameof(RequestTime)) + RequestTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(Week)) + Week + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(RequestMethod)) + RequestMethod + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(RequestPath)) + RequestPath + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(RequestQuery)) + RequestQuery + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(RequestBody)) + RequestBody + Environment.NewLine +

                GetMemberRemark<AccessLogInfo>(nameof(ResponseTime)) + ResponseTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(WorkTime)) + WorkTime + Environment.NewLine +
                GetMemberRemark<AccessLogInfo>(nameof(ResponseData)) + ResponseData + Environment.NewLine;
        }
    }
}