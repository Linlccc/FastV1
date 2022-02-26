using AutoMapper;
using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.ViewModels;
using OperateModels;
using SqlSugar;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 用户
    /// </summary>
    [ApiController, CustomRoute]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserInfo _userInfo;
        private readonly IUserClient _userClient;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserInfo userInfo, IUserClient userClient)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfo = userInfo;
            _userClient = userClient;
        }

        #region 查询

        /// <summary>
        /// 分页获取用户信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">一页大小</param>
        /// <returns></returns>
        public async Task<Msg<PageMsg<User>>> GetPageList(string name, int pageIndex = 1, int pageSize = 10)
        {
            return MsgHelper.Success(await _userClient.PageList(name, pageIndex, pageSize));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<Msg<UserView>> UserInfo()
        {
            User user = await _userClient.QueryUserRolesById(_userInfo.ID.OToLong());
            user.Roles = user.Roles.Where(r => r.Enabled).ToList();
            if (user == null) return MsgHelper.Fail<UserView>("获取信息失败，请重新登录");
            UserView userView = _mapper.Map<UserView>(user);
            return MsgHelper.Success(userView);
        }

        #endregion 查询

        #region 操作

        /// <summary>
        /// 添加/修改用户
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回是否成功</returns>
        public async Task<Msg<bool>> Oper(User user)
        {
            //判断登录名重复
            if (await _userClient.IsAnyAsync(u => u.LoginName == user.LoginName && u.Id != user.Id && u.Enabled))
                return MsgHelper.Fail("已存在该登录名", false);

            if (!await _userClient.IsAnyAsync(u => u.Id == user.Id))
            {
                user.Enabled = true;
                user.LastLoginTime = DateTime.MinValue;
                user.CreateTime = DateTime.Now;
            }
            user.LoginPWD = user.PlaintextPwd.MD5Encrypt32();
            user.UpdateTime = DateTime.Now;
            if (await _userClient.StorageableAsync(user)) return MsgHelper.Success(true);
            return MsgHelper.Fail("操作失败，请重试", false);
        }

        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="enable">是否启用</param>
        /// <returns></returns>
        public async Task<Msg<bool>> EditStatus(long id, bool enable)
        {
            if (await _userClient.SetColumnAsync(u => u.Enabled == enable, u => u.Id == id)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="avatarPath">头像路劲</param>
        /// <returns>返回新的头像的路劲</returns>
        public async Task<Msg<string>> EditUserAvatar(long id, string avatarPath)
        {
            if (id == 0) return MsgHelper.Fail<string>("没有用户信息");
            if (avatarPath.IsNull()) return MsgHelper.Fail<string>("没有头像信息");
            //移除文件开头的路径分隔符
            avatarPath = Regex.Replace(avatarPath, @"^\\", "");
            string avatarFullPath = Path.Combine(FileWellFolderInfo.WebRootPath, avatarPath);
            if (!System.IO.File.Exists(avatarFullPath)) return MsgHelper.Fail<string>("头像不存在");

            //用户头像文件夹
            string avatarFolderPath = Path.Combine(FileWellFolderInfo.WebRootPath, FileWellFolderInfo.UserAvatarFolder);
            if (!Directory.Exists(avatarFolderPath)) Directory.CreateDirectory(avatarFolderPath);

            //新头像名称
            string avatarName = DateTime.Now.Ticks.ToString() + id.ToString() + FileHelper.GetPostfix(avatarPath);
            //新头像完整名称
            string avatarFullName = Path.Combine(avatarFolderPath, avatarName);
            //保存到数据库的头像路劲
            string avatarDbName = Path.Combine(FileWellFolderInfo.UserAvatarFolder, avatarName);
            System.IO.File.Copy(avatarFullPath, avatarFullName);

            //更新数据库头像路劲
            if (await _userClient.UpdateColumnsAsync(new User() { Id = id, Avatar = avatarDbName }, u => u.Avatar))
                return MsgHelper.Success(avatarDbName);
            return MsgHelper.Fail<string>();
        }

        /// <summary>
        /// 修改自己头像
        /// </summary>
        /// <param name="avatarPath">头像路劲</param>
        /// <returns>返回新的头像的路劲</returns>
        [Authorize]
        public async Task<Msg<string>> EditAvatar(string avatarPath) => await EditUserAvatar(_userInfo.ID.OToLong(), avatarPath);

        /// <summary>
        /// 修改自己的用户信息
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<Msg<bool>> EditInfo(string nickName)
        {
            if (await _userClient.SetColumnAsync(u => u.NickName == nickName, u => u.Id == _userInfo.ID.OToLong(0)))
                return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        /// <summary>
        /// 修改自己的密码
        /// </summary>
        /// <param name="editPass"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<Msg<bool>> EditPass(EditPass editPass)
        {
            if (editPass.OldPass == editPass.NewPass) return MsgHelper.Fail<bool>("新密码不能与旧密码一致");
            if (!await _userClient.IsAnyAsync(u => u.Id == _userInfo.ID.OToLong(0) && u.LoginPWD == editPass.OldPass.MD5Encrypt32()))
                return MsgHelper.Fail("旧密码错误", false);

            User user = new()
            {
                Id = _userInfo.ID.OToLong(0),
                LoginPWD = editPass.NewPass.MD5Encrypt32(),
                PlaintextPwd = editPass.NewPass
            };
            if (await _userClient.UpdateColumnsAsync(user, u => new { u.LoginPWD, u.PlaintextPwd }))
                return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>();
        }

        #endregion 操作

        #region 删除

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Msg<bool>> Delete(long id)
        {
            if (await _userClient.DeleteByIdAsync(id)) return MsgHelper.Success(true);
            return MsgHelper.Fail<bool>("操作失败，请重试", false);
        }

        #endregion 删除
    }
}