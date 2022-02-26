using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.ViewModels;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 角色
    /// </summary>
    [ApiController, CustomRoute]
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IMapper _mapper;
        private readonly ISqlSugarClient _sugarClient;
        private readonly IRoleClient _roleClient;

        public RoleController(ILogger<RoleController> logger, IMapper mapper, ISqlSugarClient sugarClient, IRoleClient roleClient)
        {
            _logger = logger;
            _mapper = mapper;
            _sugarClient = sugarClient;
            _roleClient = roleClient;
        }

        #region 查询

        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Msg<PageMsg<Role>>> GetPageList(string name, int pageIndex = 1, int pageSize = 10)
        {
            return MsgHelper.Success(await _roleClient.QueryPageAsync(r => r.Name.Contains(name), pageIndex, pageSize, r => r.CreateTime, OrderByType.Desc));
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<List<RoleView>>> GetAllRole()
        {
            List<Role> roles = await _roleClient.QueryAsync();
            List<RoleView> roleViews = _mapper.Map<List<RoleView>>(roles);
            return MsgHelper.Success(roleViews);
        }

        #endregion 查询

        #region 操作

        /// <summary>
        /// 操作角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Oper(Role role)
        {
            if (await _roleClient.StorageableAsync(role)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        /// <summary>
        /// 修改角色状态
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="enable">角色状态</param>
        /// <returns></returns>
        public async Task<Msg<bool>> EditStatus(long id, bool enable)
        {
            if (await _roleClient.SetColumnAsync(r => r.Enabled == enable, r => r.Id == id)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 操作

        #region 删除

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Delete(long id)
        {
            if (await _roleClient.DeleteByIdAsync(id)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 删除
    }
}