using Extensions.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.Authentication_Authorization
{
    public static class Authentication_CustomSetup
    {
        /// <summary>
        /// 自定义登录认证（验证）
        /// </summary>
        public static void AddAuthentication_CustomSetup(this IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                //指定默认，认证方案
                o.AddScheme<CustomAuthenticationHandler>(nameof(CustomAuthenticationHandler), nameof(CustomAuthenticationHandler));
                o.DefaultScheme = nameof(CustomAuthenticationHandler);

                //默认异常返回处理方案
                o.AddScheme<DefaultAuthExceptionHandler>(nameof(DefaultAuthExceptionHandler), nameof(DefaultAuthExceptionHandler));
                o.DefaultChallengeScheme = nameof(DefaultAuthExceptionHandler);      //默认挑战计划（401）
                o.DefaultForbidScheme = nameof(DefaultAuthExceptionHandler);         //默认禁止方案（403）
            });
        }
    }
}