using Extensions.Auth;
using FastTool.GlobalVar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.Authentication_Authorization
{
    /// <summary>
    /// 系统 授权服务 配置
    /// </summary>
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            // 以下四种常见的授权方式。

            // 1、这个很简单，其他什么都不用做， 只需要在API层的controller上边，增加特性即可
            // [Authorize(Roles = "Admin,System")]

            // 2、这个和上边的异曲同工，好处就是不用在controller中，写多个 roles 。
            // 然后这么写 [Authorize(Policy = "Admin")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System"));
                options.AddPolicy("A_S_O", policy => policy.RequireRole("Admin", "System", "Others"));
            });

            PermissionRequirement permissionRequirement = new();
            // 3、自定义复杂的策略授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Authentication_AuthorizationInfo.Name, policy => policy.Requirements.Add(permissionRequirement));
            });

            // 4、基于Scope策略授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Scope_BlogModule_Policy", builder =>
                {
                    //客户端Scope中包含blog.core.api.BlogModule才能访问
                    builder.RequireScope("blog.core.api.BlogModule");
                });
            });

            // 注入权限处理器
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
        }
    }
}