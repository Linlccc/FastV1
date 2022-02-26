using FastSubsidiary.Hubs.ChatRoom;
using FastTool.GlobalVar;
using Microsoft.Extensions.Logging;
using Model.Models;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR
{
    /// <summary>
    /// 这些方法都是就客户端来调用的
    /// </summary>
    public class ChatHub : Hub<IChatClient>
    {
        /// <summary>
        /// 组信息（组id，组名，创建人）
        /// </summary>
        public static GroupInfos GroupInfos = new();

        private readonly ILogger<ChatHub> _logger;
        private readonly IUserClient _userClient;

        public ChatHub(ILogger<ChatHub> logger, IUserClient userClient)
        {
            _logger = logger;
            _userClient = userClient;
        }

        #region 连接相关方法

        /// <summary>
        /// 当连接建立时运行
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            //用户上线处理
            await UserOnLine();

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 当链接断开时运行
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            //用户下线处理
            await UserOffLine();

            await base.OnDisconnectedAsync(ex);
        }

        #endregion 连接相关方法

        #region 组相关方法

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task CreateGroup(string groupName)
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;
            //添加到组信息
            GroupInfo groupInfo = new(userId.Value, groupName);
            GroupInfos.Groups.Add(groupInfo);
            //创建组
            await Groups.AddToGroupAsync(Context.ConnectionId, groupInfo.GroupId.ToString());

            //通知创建成功
            await AdminChatRoom(new MsgInfo(userId.Value, GroupInfos.AdminGroup.GroupId, ToType.Group, groupInfo, DataType.CreateGroup));
        }

        /// <summary>
        /// 解散组
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task DisbandGroup(long groupId)
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            GroupInfo groupInfo = GroupInfos.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (groupInfo == null) return;

            GroupInfos.Groups.Remove(groupInfo);

            await AdminChatRoom(new MsgInfo(userId.Value, GroupInfos.AdminGroup.GroupId, ToType.Group, groupInfo, DataType.DisbandGroup));
        }

        /// <summary>
        /// 加入指定组
        /// </summary>
        /// <param name="groupId">组id</param>
        /// <returns></returns>
        public async Task JoinGroup(long groupId)
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            GroupInfo groupInfo = GroupInfos.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (groupInfo == null) return;
            if (!groupInfo.UserIds.Contains(userId.Value)) groupInfo.UserIds.Add(userId.Value);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupInfo.GroupId.ToString());
            //通知添加成功
            await AdminChatRoom(new MsgInfo(userId.Value, groupInfo.GroupId, ToType.Group, "", DataType.JoinGroup));
        }

        /// <summary>
        /// 退出指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task RemoveGroup(long groupId)
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            GroupInfo groupInfo = GroupInfos.Groups.FirstOrDefault(g => g.GroupId == groupId);
            if (groupInfo == null) return;
            groupInfo.UserIds.Remove(userId.Value);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupInfo.GroupId.ToString());

            await AdminChatRoom(new MsgInfo(userId.Value, groupInfo.GroupId, ToType.Group, "", DataType.PartGroup));
        }

        #endregion 组相关方法

        #region 后台聊天室

        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <returns></returns>
        public async Task GetAllUserInfo()
        {
            var userInfos = await _userClient.SugarClient.Queryable<User>()
                .Select(u => new
                {
                    u.AdminSignalrConnectionId,
                    u.Avatar,
                    u.Id,
                    u.NickName
                }).ToListAsync();
            await AdminChatRoom(new MsgInfo(0, 0, ToType.Caller, userInfos, DataType.AllUserInfo));
        }

        /// <summary>
        /// 获取当前全部组
        /// </summary>
        /// <returns></returns>
        public async Task GetAllGroupInfo() => await AdminChatRoom(new MsgInfo(0, 0, ToType.Caller, GroupInfos.Groups, DataType.AllGroupInfo));

        /// <summary>
        /// 后台聊天室发送消息
        /// </summary>
        /// <param name="toId">接收id</param>
        /// <param name="toType">接收类型</param>
        /// <param name="data">数据</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public async Task AdminChatRoomSendMsg(long toId, ToType toType, object data, DataType dataType)
        {
            long? id = GetCurrentUserId();
            if (id.HasValue) await AdminChatRoom(new MsgInfo(id.Value, toId, toType, data, dataType));
        }

        /// <summary>
        /// 后台聊天室使用
        /// </summary>
        /// <param name="msgInfo">发送信息</param>
        /// <returns></returns>
        private async Task AdminChatRoom(MsgInfo msgInfo)
        {
            IChatClient transmitter = msgInfo.ToType switch
            {
                ToType.Caller => Clients.Caller,
                ToType.User => (await _userClient.QueryByIdAsync(msgInfo.ToId)).AdminSignalrConnectionId is string toUserConnectionId && toUserConnectionId.IsNNull() ? Clients.Client(toUserConnectionId) : null,
                ToType.Group => Clients.Group(msgInfo.ToId.ToString()),
                ToType.Others => Clients.Others,
                ToType.OthersInGroup => Clients.OthersInGroup(msgInfo.ToId.ToString()),
                ToType.All => Clients.All,
                _ => null
            };
            if (transmitter.IsNull())
            {
                _logger.LogError($"发送失败，发送信息：{JsonConvert.SerializeObject(msgInfo)}");
                return;
            }

            await transmitter.AdminChatRoom(msgInfo);
        }

        #endregion 后台聊天室

        #region 后台获取日志

        /// <summary>
        /// 客户端获取 访问日志信息
        /// </summary>
        /// <param name="startTime">开始日志时间</param>
        /// <param name="endTime">最后日志时间</param>
        /// <param name="count">要取得日志条数</param>
        /// <returns></returns>
        public async Task GetAdminLogs(string logType, DateTime startTime, DateTime endTime, int count)
        {
            //按照日志类型获取日志
            object result = logType switch
            {
                LogFolderInfo.AccessFolder => LogHelper.GetLogInfos<AccessLogInfo>(startTime, endTime, count),//访问日志
                LogFolderInfo.DbOperFolder => LogHelper.GetLogInfos<DbOperLogInfo>(startTime, endTime, count),
                LogFolderInfo.SqlLogFolder => LogHelper.GetLogInfos<SqlLogInfo>(startTime, endTime, count),
                LogFolderInfo.ErrorSqlLogFolder => LogHelper.GetLogInfos<ErrorSqlLogInfo>(startTime, endTime, count),
                _ => new List<object>()
            };
            await Clients.Caller.AdminLog(logType, true, result);
        }

        #endregion 后台获取日志

        #region 帮助

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <returns></returns>
        private async Task UserOnLine()
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;
            // 修改用户连接id
            await _userClient.SetColumnAsync(u => u.AdminSignalrConnectionId == Context.ConnectionId, u => u.Id == userId);

            // 将该用户加入本就存在的组
            GroupInfos.Groups.Where(g => g.UserIds.Contains(userId.Value)).ToList().ForEach(async g => await Groups.AddToGroupAsync(Context.ConnectionId, g.GroupId.ToString()));

            //通知用户上线
            await AdminChatRoom(new MsgInfo(userId.Value, 0, ToType.All, Context.ConnectionId, DataType.UserOnLine));
        }

        /// <summary>
        /// 用户下线
        /// </summary>
        /// <returns></returns>
        private async Task UserOffLine()
        {
            long? userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            // 清除用户连接id
            await _userClient.SetColumnAsync(u => u.AdminSignalrConnectionId == null, u => u.Id == userId);

            // 将该用户移除存在的组,这个不用，如果不是自己退出就不退出
            //GroupInfos.Groups.Where(g => g.UserIds.Contains(userId.Value)).ToList().ForEach(async g => await Groups.RemoveFromGroupAsync(Context.ConnectionId, g.GroupId.ToString()));

            //通知用户下线
            await AdminChatRoom(new MsgInfo(userId.Value, 0, ToType.All, null, DataType.UserOffLine));
        }

        /// <summary>
        /// 当前用户id
        /// </summary>
        /// <returns></returns>
        private long? GetCurrentUserId() => Context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value.OToLong();

        /// <summary>
        /// 当前用户名
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUseName() => Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        #endregion 帮助
    }
}