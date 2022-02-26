namespace FastSubsidiary.Hubs.ChatRoom
{
    /// <summary>
    /// 消息信息
    /// </summary>
    public class MsgInfo
    {
        public MsgInfo()
        { }

        /// <summary>
        /// 消息信息
        /// </summary>
        /// <param name="fromUserId">发送用户id</param>
        /// <param name="toId">接收id</param>
        /// <param name="toType">接收类型</param>
        /// <param name="data">发送数据</param>
        /// <param name="dataType">发送数据类型</param>
        public MsgInfo(long fromUserId, long toId, ToType toType, object data, DataType dataType)
        {
            FromUserId = fromUserId;
            ToId = toId;
            ToType = toType;
            Data = data;
            DataType = dataType;
        }

        /// <summary>
        /// 发送人id
        /// </summary>
        public long FromUserId { get; set; }

        /// <summary>
        /// 接收id
        /// </summary>
        public long ToId { get; set; }

        /// <summary>
        /// 接收类型
        /// </summary>
        public ToType ToType { get; set; }

        /// <summary>
        /// 发送的数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType { get; set; }
    }

    /// <summary>
    /// 发送到哪的类型
    /// </summary>
    public enum ToType
    {
        /// <summary>
        /// 自己
        /// </summary>
        Caller = 1,

        /// <summary>
        /// 用户
        /// </summary>
        User,

        /// <summary>
        /// 组
        /// </summary>
        Group,

        /// <summary>
        /// 其他人
        /// </summary>
        Others,

        /// <summary>
        /// 在某个组的其他人
        /// </summary>
        OthersInGroup,

        /// <summary>
        /// 全部
        /// </summary>
        All
    }

    /// <summary>
    /// 发送数据的类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,

        /// <summary>
        /// 图片
        /// </summary>
        Image,

        /// <summary>
        /// 用户上线
        /// </summary>
        UserOnLine,

        /// <summary>
        /// 用户下线
        /// </summary>
        UserOffLine,

        /// <summary>
        /// 创建组
        /// </summary>
        CreateGroup,

        /// <summary>
        /// 解散组
        /// </summary>
        DisbandGroup,

        /// <summary>
        /// 加入组
        /// </summary>
        JoinGroup,

        /// <summary>
        /// 退出组
        /// </summary>
        PartGroup,

        /// <summary>
        /// 组信息
        /// </summary>
        GroupInfo,

        /// <summary>
        /// 所有组信息
        /// </summary>
        AllGroupInfo,

        /// <summary>
        /// 用户信息
        /// </summary>
        UserInfo,

        /// <summary>
        /// 所有用户信息
        /// </summary>
        AllUserInfo,
    }
}