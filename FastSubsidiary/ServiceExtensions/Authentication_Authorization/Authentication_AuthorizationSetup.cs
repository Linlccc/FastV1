using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Extensions.ServiceExtensions.Authentication_Authorization
{
    public static class Authentication_AuthorizationSetup
    {
        public static void AddAuthentication_AuthorizationSetup(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            if (Authentication_AuthorizationInfo.UseAuthenticationSchemeName == Authentication_AuthorizationInfo.AuthenticationSchemeName.Ids4)
                services.AddAuthentication_Ids4Setup();  //Ids4 认证
            else if (Authentication_AuthorizationInfo.UseAuthenticationSchemeName == Authentication_AuthorizationInfo.AuthenticationSchemeName.Jwt)
                services.AddAuthentication_JWTSetup();  //JWT 认证
            else
                services.AddAuthentication_CustomSetup();//自定义登录认证
            services.AddAuthorizationSetup();       //身份验证规则

            services.AddScoped<IUserInfo, UserInfo>();    //用户信息类
        }
    }
}