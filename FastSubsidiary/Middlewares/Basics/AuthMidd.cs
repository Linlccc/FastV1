using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Extensions.Middlewares.Basics
{
    /// <summary>
    /// 测试用户用于测试环境使用
    /// 避免频繁登录
    /// </summary>
    public class AuthMidd
    {
        private readonly RequestDelegate _next;

        private string _currentUserId;

        private string _currentRoleName;

        public AuthMidd(RequestDelegate next)
        {
            _next = next;
            _currentUserId = null;
            _currentRoleName = null;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path;
            // 请求地址，通过Url参数的形式，设置用户id和rolename
            if (path == "/noauth")
            {
                string userid = context.Request.Query["userid"];//获取请求的id
                if (userid.IsNNull()) _currentUserId = userid;

                string rolename = context.Request.Query["rolename"];//获取角色
                if (rolename.IsNNull()) _currentRoleName = rolename;

                await SendOkResponse(context, $"用户设置为{_currentUserId}，角色设置为{_currentRoleName}");
                return;
            }
            else if (path == "/noauth/d")
            {
                _currentUserId = AppConfig.GetNode("Middleware", "TestAuthUser", "TestUserId");
                _currentRoleName = AppConfig.GetNode("Middleware", "TestAuthUser", "TestUserRole");
                await SendOkResponse(context, $"使用默认用户，id为{_currentUserId}，角色为{_currentRoleName}");
                return;
            }
            // 重置角色信息
            else if (path == "/noauth/reset")
            {
                _currentUserId = null;
                _currentRoleName = null;
                await SendOkResponse(context, $"当前无测试用户， 受保护的端点需要令牌");
                return;
            }
            else
            {
                string currentUserId = _currentUserId;
                string currentRoleName = _currentRoleName;

                // 如果用户id和rolename都不为空
                // 可以配置HttpContext.User信息了，也就相当于登录了。
                if (currentUserId.IsNNull() && currentRoleName.IsNNull())
                {
                    ClaimsIdentity user = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name,"Test user"),
                        new Claim(ClaimTypes.NameIdentifier,"TestNameIdentifier"),//用户标识
                        new Claim(JwtRegisteredClaimNames.Jti,currentUserId),//id
                        new Claim(ClaimTypes.Role,currentRoleName),//角色名称
                        new Claim(ClaimTypes.Expiration,DateTime.Now.AddDays(1).ToString()),//过期时间
                        new Claim("http://schemas.microsoft.com/identity/claims/identityprovider", "ByPassAuthMiddleware"),//身份提供者
                        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname","User"),
                        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname","Microsoft")
                    }, "Auth");
                    context.User = new ClaimsPrincipal(user);
                }
            }
            await _next.Invoke(context);

            // 设置返回信息
            static async Task SendOkResponse(HttpContext context, string message)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                context.Response.ContentType = "text/plain;charset=utf-8";
                await context.Response.WriteAsync(message);
            }
        }
    }
}