using Extensions.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.ViewModels;
using OperateModels;
using SqlSugar;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 身份
    /// </summary>
    [ApiController, CustomRoute]
    public class IdentityController : Controller
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly PermissionRequirement _permission;
        private readonly IUserClient _userClient;

        public IdentityController(ILogger<IdentityController> logger, PermissionRequirement permission, IUserClient userClient)
        {
            _logger = logger;
            _permission = permission;
            _userClient = userClient;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginInfo">用户信息</param>
        /// <returns></returns>
        [HttpPost("/api/Login")]
        public async Task<Msg<TokenInfoView>> Login(LoginInfo loginInfo)
        {
            User user = await _userClient.SingleAsync(u => u.Enabled && u.LoginName == loginInfo.Name && u.LoginPWD == loginInfo.Password.MD5Encrypt32());
            if (user.IsNull()) return MsgHelper.Fail<TokenInfoView>();

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,user.NickName),
                new Claim(ClaimTypes.NameIdentifier,user.NickName),
                new Claim(JwtRegisteredClaimNames.Name,user.LoginName),
                new Claim(JwtRegisteredClaimNames.Jti,user.Id.OToString()),
                new Claim(ClaimTypes.Expiration,DateTime.Now.Add(_permission.Expiration).ToString())//过期时间
            };
            TokenInfoView tokenInfoViewModel = JwtToken.BuildJwtToken(claims, _permission);
            await _userClient.SetColumnAsync(u => u.LastLoginTime == DateTime.Now, u => u.Id == user.Id);//更新最后登录时间
            return MsgHelper.Success(tokenInfoViewModel);
        }

        /// <summary>
        /// 刷新登录状态
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/RefreshLogin")]
        public async Task<Msg<TokenInfoView>> RefreshLogin([FromBody] string token)
        {
            if (token.IsNull()) return MsgHelper.Fail<TokenInfoView>("认证失败");

            TokenModelJwt userInfo = JwtToken.SerializeJwt(token);
            User user = await _userClient.SingleAsync(u => u.Enabled && u.Id == userInfo.Uid);
            if (user.IsNull()) return MsgHelper.Fail<TokenInfoView>();

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,user.NickName),
                new Claim(ClaimTypes.NameIdentifier,user.NickName),
                new Claim(JwtRegisteredClaimNames.Name,user.LoginName),
                new Claim(JwtRegisteredClaimNames.Jti,user.Id.OToString()),
                new Claim(ClaimTypes.Expiration,DateTime.Now.Add(_permission.Expiration).ToString())//过期时间
            };
            TokenInfoView tokenInfoViewModel = JwtToken.BuildJwtToken(claims, _permission);
            await _userClient.SetColumnAsync(u => u.LastLoginTime == DateTime.Now, u => u.Id == user.Id);//更新最后登录时间
            return MsgHelper.Success(tokenInfoViewModel);
        }
    }
}