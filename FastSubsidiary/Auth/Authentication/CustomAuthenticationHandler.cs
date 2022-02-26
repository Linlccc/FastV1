using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Extensions.Auth
{
    /// <summary>
    /// 自定义身份验证处理程序
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 登录状态判断
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (true)
            {
                return await Task.FromResult(AuthenticateResult.Fail("not login"));//没登录
            }
            if (false)
            {
                return await Task.FromResult(AuthenticateResult.NoResult());//未处理（没结果）
            }

            var a = Request.Headers;

            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Name,"TestName"),
                new Claim(ClaimTypes.Name,"TestName")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));//以登录
        }
    }
}