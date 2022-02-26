using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Extensions.ServiceExtensions.Authentication_Authorization
{
    public static class Authentication_JWTSetup
    {
        /// <summary>
        /// JWT权限认证（验证）
        /// </summary>
        public static void AddAuthentication_JWTSetup(this IServiceCollection services)
        {
            // 令牌验证参数
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,    //验证发行人的签名秘钥
                IssuerSigningKey = JwtConfig.SecurityKey,      //发行人签名密钥
                ValidateIssuer = true,              //验证发行人
                ValidIssuer = JwtConfig.Issuer,     //发行人
                ValidateAudience = true,            //验证码订阅人
                ValidAudience = JwtConfig.Audience, //订阅人
                ValidateLifetime = true,            //验证生命周期
                ClockSkew = TimeSpan.FromSeconds(30),   //获取或设置验证时间时要应用的时钟偏斜
                RequireExpirationTime = true,       //是否需要到期时间
            };

            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                //默认认证方案
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                //默认异常返回处理方案
                o.AddScheme<DefaultAuthExceptionHandler>(nameof(DefaultAuthExceptionHandler), nameof(DefaultAuthExceptionHandler));
                o.DefaultChallengeScheme = nameof(DefaultAuthExceptionHandler);      //默认挑战计划（401）
                o.DefaultForbidScheme = nameof(DefaultAuthExceptionHandler);         //默认禁止方案（403）
            })
            .AddJwtBearer(o =>// 添加JwtBearer服务
            {
                o.TokenValidationParameters = tokenValidationParameters;           //令牌验证参数
                o.EventsType = typeof(CustomJwtBearerEvents);
            });

            services.AddSingleton<CustomJwtBearerEvents>();
        }
    }

    /// <summary>
    /// 自定义Jwt认证
    /// </summary>
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        /// <summary>
        /// 认证身份时调用
        /// 可以自己配置token从哪获取到
        /// 默认是 Heaher 中的 Authorization
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task MessageReceived(MessageReceivedContext context)
        {
            //从头中获取，其实默认也是这样
            //context.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //设置 SignalR 身份
            string token = context.Request.Query["access_token"];//获取路径中的token信息
            if (context.HttpContext.Request.Path.StartsWithSegments("/chathub") && token.IsNNull()) context.Token = token;

            return base.MessageReceived(context);
        }

        /// <summary>
        /// 在请求处理期间引发异常时调用(验证登录信息错误)
        /// 例外情况 除非受到抑制
        /// 否则在此事件发生后将其重新抛出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            string token = context.Request.Headers["Authorization"].OToString().Replace("Bearer ", "");//获取身份加密字符串
            JwtSecurityToken jwtToken = token.IsNull() ? new() : new(token);

            if (jwtToken.Issuer != JwtConfig.Issuer)//发行人
            {
                context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
            }

            if (jwtToken.Audiences.FirstOrDefault() != JwtConfig.Audience)//订阅人
            {
                context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
            }

            // 如果过期，则把<是否过期>添加到，返回头信息中
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Access-Control-Expose-Headers", "Token-Expired");//暴露指定响应头
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return base.AuthenticationFailed(context);
        }

        /// <summary>
        /// 认证成功后触发
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenValidated(TokenValidatedContext context)
        {
            return base.TokenValidated(context);
        }

        /// <summary>
        /// 认证失败时触发（401）
        /// 有专门写失败处理，没用这个
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Challenge(JwtBearerChallengeContext context)
        {
            context.Response.Headers.Add("Token-Error", context.ErrorDescription);
            return base.Challenge(context);
        }

        /// <summary>
        /// 在授权失败时调用（403）
        /// 有专门写失败处理，没用这个
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Forbidden(ForbiddenContext context)
        {
            context.Response.Headers.Add("Power-Error", "true");
            return base.Forbidden(context);
        }
    }
}