using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Extensions.Auth
{
    /// <summary>
    /// 默认认证错误处理程序
    /// 不要将该处理程序当做认证程序，100%认证成功哦
    /// </summary>
    public class DefaultAuthExceptionHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public DefaultAuthExceptionHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 这个的认证方案没使用
        /// 只要过这个认证方案的都成功
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Name,"Fake user"),
                new Claim(JwtRegisteredClaimNames.Jti,"Fake user"),
                new Claim(ClaimTypes.Name,"Fake user")
            };
            ClaimsIdentity identity = new(claims, Scheme.Name);
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));//返回登录成功
        }

        /// <summary>
        /// 没有登录 401 错误
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(JsonConvert.SerializeObject(MsgHelper.Fail<string>("很抱歉，您无权访问该接口，请确保已经登录!", code: StatusCodes.Status401Unauthorized)));
        }

        /// <summary>
        /// 没有权限 403 错误
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync(JsonConvert.SerializeObject(MsgHelper.Fail<string>("很抱歉，您的访问权限等级不够，联系管理员!", code: StatusCodes.Status403Forbidden)));
        }
    }
}