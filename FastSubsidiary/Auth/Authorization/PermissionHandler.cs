using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extensions.Auth
{
    /// <summary>
    /// 自定义的权限授权处理器
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleFeatureClient _roleFeaturesDb;
        private readonly IUserClient _userInfoDb;

        public PermissionHandler(IAuthenticationSchemeProvider authenticationSchemeProvider, IHttpContextAccessor httpContextAccessor, IRoleFeatureClient roleFeaturesDb, IUserClient userInfoDb)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _httpContextAccessor = httpContextAccessor;
            _roleFeaturesDb = roleFeaturesDb;
            _userInfoDb = userInfoDb;
        }

        /// <summary>
        /// 重写异步处理程序
        /// 根据特定要求来决定是否允许授权。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //context.Succeed(requirement);
            //return;

            //如果没有验证规则 获取验证规则
            if (!requirement.Permissions.Any())
            {
                //List<RoleFeature> roleFeatures = await _roleFeaturesDb.GetRoleFeatures();
                //roleFeatures.ForEach(rf =>
                //{
                //    requirement.Permissions.Add(new PermissionItem()
                //    {
                //        Role = rf.Role.Name,
                //        Url = rf.Features.InterfaceInfo.LinkUrl
                //    });
                //});
            }

            //封装有关单个HTTP请求的所有特定于HTTP的信息。
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            #region 一套默认的认证(这个还不太清楚)
            // 整体结构类似认证中间件UseAuthentication的逻辑，具体查看开源地址
            // https://github.com/dotnet/aspnetcore/blob/master/src/Security/Authentication/Core/src/AuthenticationMiddleware.cs
            httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = httpContext.Request.Path,
                OriginalPathBase = httpContext.Request.PathBase
            });
            // Give any IAuthenticationRequestHandler schemes a chance to handle the request
            // 主要作用是: 判断当前是否需要进行远程验证，如果是就进行远程验证
            IAuthenticationHandlerProvider handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();//获取认证处理程序
            foreach (AuthenticationScheme scheme in await _authenticationSchemeProvider.GetRequestHandlerSchemesAsync())//以优先级顺序返回方案以进行请求处理
            {
                if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())//如果请求应该停止返回true
                {
                    context.Fail();
                    return;
                }
            }
            #endregion 一套默认的认证(这个还不太清楚)

            //得到数据
            string idStr = httpContext.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;   //得到用户ID
            string expiration = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration)?.Value;           //得到用户过期时间
            string NameIdentifier = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;   //得到名称标识符，用于判断是否是测试用户

            //如果没有过期时间 或者 过期时间小于当前时间
            if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(expiration) || DateTime.Parse(expiration) < DateTime.Now)//判断过期时间这个后面要放开
            {
                context.Fail();
                return;
            }

            //如果是测试用户
            if (NameIdentifier == "TestNameIdentifier")
            {
                context.Succeed(requirement);
                return;
            }

            //得到用户的角色名集合
            //List<string> rolenames = await _userInfoDb.GetRoleNamesById(long.Parse(idStr));
            List<string> rolenames = new();
            //得到这个用户的角色和能访问的接口连接
            List<PermissionItem> currentUser_RoleAndInterface = requirement.Permissions.Where(p => rolenames.Contains(p.Role)).ToList();
            //得到请求的路径
            string questUrl = httpContext.Request.Path.Value.ToLower();
            //判断是否有和当前请求匹配的权限
            bool isMatch = false;
            foreach (PermissionItem item in currentUser_RoleAndInterface)
            {
                if (Regex.Match(questUrl, item.Url.ToLower())?.Value == questUrl)
                {
                    isMatch = true;
                    break;
                }
            }

            //如果有匹配的
            if (isMatch) context.Succeed(requirement);
            else context.Fail();

            return;
        }
    }
}