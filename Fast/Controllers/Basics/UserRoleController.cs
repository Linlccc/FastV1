using AutoMapper;
using Extensions.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [ApiController, CustomRoute]
    public class UserRoleController : Controller
    {
        private readonly ILogger<UserRoleController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserInfo _userInfo;
        private readonly IUserRoleClient _userRoleClient;

        public UserRoleController(ILogger<UserRoleController> logger, IMapper mapper, IUserInfo userInfo, IUserRoleClient userRoleClient)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfo = userInfo;
            _userRoleClient = userRoleClient;
        }

        /// <summary>
        /// 设置用户的角色
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="roleIds">角色id集合</param>
        /// <returns></returns>
        public async Task<Msg<bool>> SetUserRoles(long userId, long[] roleIds)
        {
            if (userId == 0) return MsgHelper.Fail<bool>("必须有用户id");
            if (await _userRoleClient.SetUserRoles(userId, roleIds)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }
    }
}