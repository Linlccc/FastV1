using AutoMapper;
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
    /// 角色接口
    /// </summary>
    [ApiController, CustomRoute]
    public class RoleInterfaceController
    {
        private readonly ILogger<RoleInterfaceController> _logger;
        private readonly IMapper _mapper;
        private readonly IRoleInterfaceClient _roleInterfaceClient;

        public RoleInterfaceController(ILogger<RoleInterfaceController> logger, IMapper mapper, IRoleInterfaceClient roleInterfaceClient)
        {
            _logger = logger;
            _mapper = mapper;
            _roleInterfaceClient = roleInterfaceClient;
        }

        /// <summary>
        /// 设置角色接口
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="interfaceIds"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> SetRoleInterfaces(long roleId, long[] interfaceIds)
        {
            if (roleId == 0) return MsgHelper.Fail<bool>("必须有角色id");
            if (await _roleInterfaceClient.SetRoleInterfaces(roleId, interfaceIds)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        /// <summary>
        /// 获取角色的接口id集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Msg<List<long>>> GetRoleInterfaceIds(long roleId)
        {
            List<long> interfaceIds = await _roleInterfaceClient.SugarClient.Queryable<RoleInterface>()
                .Where(ri => ri.RoleId == roleId)
                .Select(ri => ri.InterfaceId)
                .ToListAsync();
            return MsgHelper.Success(interfaceIds);
        }
    }
}