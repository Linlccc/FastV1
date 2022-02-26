using System.Collections.Generic;
using System.Linq;
using Yitter.IdGenerator;

namespace FastSubsidiary.Hubs.ChatRoom
{
    public class GroupInfos
    {
        private List<GroupInfo> _groupInfos = new();

        /// <summary>
        /// 组信息集合
        /// </summary>
        public List<GroupInfo> Groups
        {
            get
            {
                if (!_groupInfos.Any(g => g.GroupId == AdminGroup.GroupId)) _groupInfos.Add(AdminGroup);
                return _groupInfos;
            }
            set
            {
                _groupInfos = value;
            }
        }

        /// <summary>
        /// 后台组信息
        /// </summary>
        public GroupInfo AdminGroup { get; set; } = new GroupInfo(0, "Admin");
    }

    /// <summary>
    /// 聊天室组
    /// </summary>
    public class GroupInfo
    {
        public GroupInfo(long createUserId, string groupName)
        {
            CreateUserId = createUserId;
            GroupName = groupName;
        }

        private long _groupId;

        /// <summary>
        /// 组id（不能重复，根据该值指定组）
        /// </summary>
        public long GroupId
        {
            get
            {
                if (_groupId == 0) _groupId = YitIdHelper.NextId();
                return _groupId;
            }
            set => _groupId = value;
        }

        private List<long> _userIds = new();

        /// <summary>
        /// 组内的用户信息
        /// </summary>
        public List<long> UserIds
        {
            get
            {
                if (!_userIds.Any(u => u == CreateUserId)) _userIds.Add(CreateUserId);
                return _userIds;
            }
            set
            {
                _userIds = value;
            }
        }

        /// <summary>
        /// 创建用户id
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 组名(可重复，用户创建)
        /// </summary>
        public string GroupName { get; set; }
    }
}