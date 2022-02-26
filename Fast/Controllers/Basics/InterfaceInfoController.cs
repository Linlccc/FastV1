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
    /// 接口
    /// </summary>
    [ApiController, CustomRoute]
    public class InterfaceInfoController : Controller
    {
        private readonly ILogger<InterfaceInfoController> _logger;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IInterfaceInfoClient _interfaceInfoClient;

        public InterfaceInfoController(ILogger<InterfaceInfoController> logger, ISqlSugarClient sqlSugarClient, IInterfaceInfoClient interfaceInfoClient)
        {
            _logger = logger;
            _sqlSugarClient = sqlSugarClient;
            _interfaceInfoClient = interfaceInfoClient;
        }

        #region 查询

        /// <summary>
        /// 分页获取接口信息
        /// </summary>
        /// <param name="description">接口描述</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Msg<PageMsg<InterfaceInfo>>> GetPageList(string description, int pageIndex = 1, int pageSize = 10)
        {
            return MsgHelper.Success(await _interfaceInfoClient.QueryPageAsync(i => string.IsNullOrEmpty(description) || i.Description.Contains(description), pageIndex, pageSize));
        }

        /// <summary>
        /// 获取全部接口
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<List<InterfaceInfo>>> GetAllInterfaceInfo()
        {
            List<InterfaceInfo> interfaceInfos = await _interfaceInfoClient.QueryAsync();
            return MsgHelper.Success(interfaceInfos);
        }

        #endregion 查询

        #region 操作

        /// <summary>
        /// 操作接口
        /// </summary>
        /// <param name="interfaceInfo"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Oper(InterfaceInfo interfaceInfo)
        {
            if (!await _interfaceInfoClient.IsAnyAsync(i => i.Id == interfaceInfo.Id))
            {
                interfaceInfo.CreateTime = DateTime.Now;
            }
            if (await _interfaceInfoClient.StorageableAsync(interfaceInfo)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        /// <summary>
        /// 修改接口状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> EditStatus(long id, bool enable)
        {
            if (await _interfaceInfoClient.SetColumnAsync(i => i.Enabled == enable, i => i.Id == id)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        #endregion 操作

        #region 删除

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Delete(long id)
        {
            if (await _interfaceInfoClient.DeleteByIdAsync(id)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 删除
    }
}