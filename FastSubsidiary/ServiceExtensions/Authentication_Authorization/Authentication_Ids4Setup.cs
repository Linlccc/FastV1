using Extensions.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.Authentication_Authorization
{
    public static class Authentication_Ids4Setup
    {
        public static void AddAuthentication_Ids4Setup(this IServiceCollection services)
        {
            // 添加Identityserver4认证
            services.AddAuthentication(o =>
            {
                //默认认证方案
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                //默认异常返回处理方案
                o.AddScheme<DefaultAuthExceptionHandler>(nameof(DefaultAuthExceptionHandler), nameof(DefaultAuthExceptionHandler));
                o.DefaultChallengeScheme = nameof(DefaultAuthExceptionHandler);
                o.DefaultForbidScheme = nameof(DefaultAuthExceptionHandler);
            })
            .AddIdentityServerAuthentication(options => //IdentityServer4身份处理程序
            {
                options.Authority = AppConfig.GetNode("Certified", "IdentityServer4", "AuthorizationUrl");  //认证中心域名
                options.RequireHttpsMetadata = false;                                                       //是否需要HTTPS
                options.ApiName = AppConfig.GetNode("Certified", "IdentityServer4", "ApiName");             //用于针对自省端点进行身份验证的API资源的名称
                options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;        //指定支持哪些令牌类型（JWT，引用或两者）
                options.ApiSecret = "api_secret";                                                           //用于针对自省端点进行身份验证的机密
            });
        }
    }
}