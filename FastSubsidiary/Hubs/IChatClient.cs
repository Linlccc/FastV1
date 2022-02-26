using FastSubsidiary.Hubs.ChatRoom;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR
{
    /// <summary>
    /// 这里是客户端调用 ChatHub 类里面的方法
    /// ChatHub 使用这个接口的方法来发送信息，客户端就订阅方法名来接收信息
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        /// 服务发送信息
        /// 这个发送的信息 客户端要订阅 【ReceiveMessage】 来接收
        /// </summary>
        /// <param name="user">指定接收客户端</param>
        /// <param name="message">信息内容</param>
        /// <returns></returns>
        Task ReceiveMessage(string user, string message);

        #region 后台

        /// <summary>
        /// 发送后台一批日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="isMultiple">是否是多条日志</param>
        /// <param name="log">日志</param>
        /// <returns></returns>
        Task AdminLog(string logType, bool isMultiple, object log);

        /// <summary>
        /// 后台聊天室使用
        /// </summary>
        /// <param name="msgInfo">消息信息</param>
        /// <returns></returns>
        Task AdminChatRoom(MsgInfo msgInfo);

        #endregion 后台
    }
}