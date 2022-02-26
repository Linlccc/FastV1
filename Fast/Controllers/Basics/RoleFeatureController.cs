using AutoMapper;
using Extensions.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 角色权限（功能）
    /// </summary>
    [ApiController, CustomRoute]
    public class RoleFeatureController : Controller
    {
        private readonly ILogger<RoleFeatureController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserInfo _userInfo;
        private readonly IRoleFeatureClient _roleFeatureClient;

        public RoleFeatureController(ILogger<RoleFeatureController> logger, IMapper mapper, IUserInfo userInfo, IRoleFeatureClient roleFeatureClient)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfo = userInfo;
            _roleFeatureClient = roleFeatureClient;
        }

        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="featureIds">功能id集合</param>
        /// <returns></returns>
        public async Task<Msg<bool>> SetRoleFeatures(long roleId, long[] featureIds)
        {
            if (roleId == 0) return MsgHelper.Fail<bool>("必须有角色id");
            if (await _roleFeatureClient.SetRoleFeatures(roleId, featureIds)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        /// <summary>
        /// 获取角色的功能id列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Msg<List<long>>> GetRoleFeatureIds(long roleId)
        {
            List<long> featureIds = await _roleFeatureClient.SugarClient.Queryable<RoleFeature>()
                .Where(rf => rf.RoleId == roleId)
                .Select(rf => rf.FeaturesId)
                .ToListAsync();
            return MsgHelper.Success(featureIds);
        }
    }
}