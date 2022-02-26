using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fast.Controllers.Test
{
    /// <summary>
    /// SignalR 测试
    /// 访问 http://localhost:8080/signalr/index.html 页面
    /// </summary>
    [CustomRoute(ApiGroup.Test)]
    public class SignalRController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// 向所有客户端发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void AllSend(string msg)
        {
            _hubContext.Clients.All.SendAsync("ReceiveMessage", msg, msg).Wait();
        }

        /// <summary>
        /// 向指定组发送信息
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="msg"></param>
        public void GroupSend(string groupName, string msg)
        {
            _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", msg, msg).Wait();
        }
    }
}