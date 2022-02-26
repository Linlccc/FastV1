using Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
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
    /// 功能
    /// </summary>
    [ApiController, CustomRoute]
    public class FeatureController : Controller
    {
        private readonly ILogger<FeatureController> _logger;
        private readonly IUserInfo _userInfo;
        private readonly IFeaturesClient _featuresClient;

        public FeatureController(ILogger<FeatureController> logger, IUserInfo userInfo, IFeaturesClient featuresClient)
        {
            _logger = logger;
            _userInfo = userInfo;
            _featuresClient = featuresClient;
        }

        #region 查询

        /// <summary>
        /// 分页获取功能信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Msg<PageMsg<Feature>>> GetPageList(string name, int pageIndex = 1, int pageSize = 10)
        {
            return MsgHelper.Success(await _featuresClient.QueryPageAsync(i => i.Name.Contains(name), pageIndex, pageSize));
        }

        /// <summary>
        /// 获取全部功能信息树
        /// </summary>
        /// <returns></returns>
        public async Task<Msg<List<Feature>>> GetAllFeaturesTree()
        {
            return MsgHelper.Success(await _featuresClient.GetAllFeaturesTree());
        }

        /// <summary>
        /// 获取当前用户的权限
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<Msg<List<Feature>>> GetPermissions()
        {
            List<Feature> features = await _featuresClient.GetUserFeatureTree(_userInfo.ID.OToLong());
            return MsgHelper.Success(features);
        }

        #endregion 查询

        #region 操作

        /// <summary>
        /// 操作功能
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Oper(Feature features)
        {
            if (await _featuresClient.StorageableAsync(features)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        /// <summary>
        /// 修改功能状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> EditStatus(long id, bool enable)
        {
            if (await _featuresClient.SetColumnAsync(f => f.Enabled == enable, f => f.Id == id)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        #endregion 操作

        #region 删除

        /// <summary>
        /// 删除功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Delete(long id)
        {
            if (id == 0) return MsgHelper.Fail("删除id不能为0", false);
            if (await _featuresClient.RecursiveDeleteById(id)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        #endregion 删除
    }
}