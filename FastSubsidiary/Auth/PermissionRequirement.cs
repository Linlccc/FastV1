using FastTool.GlobalVar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;

namespace Extensions.Auth
{
    /// <summary>
    /// 必要参数类，认证的参数类
    /// 继承 IAuthorizationRequirement，用于设计自定义权限处理器PermissionHandler
    /// 因为AuthorizationHandler 中的泛型参数 TRequirement 必须继承 IAuthorizationRequirement
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 角色和访问链接权限的关系
        /// </summary>
        public List<PermissionItem> Permissions { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        public PermissionRequirement()
        {
            Permissions = new List<PermissionItem>();//这个动态绑定数据库，现在留空
            Issuer = JwtConfig.Issuer;//发行人
            Audience = JwtConfig.Audience;//订阅
            SigningCredentials = new SigningCredentials(JwtConfig.SecurityKey, SecurityAlgorithms.HmacSha256);//签名凭据
            Expiration = TimeSpan.FromSeconds(60 * 60);//接口的过期时间
        }
    }

    /// <summary>
    /// 角色和接口
    /// </summary>
    public class PermissionItem
    {
        /// <summary>
        /// 用户或角色或其他凭据名称
        /// </summary>
        public virtual string Role { get; set; }

        /// <summary>
        /// 请求Url
        /// </summary>
        public virtual string Url { get; set; }
    }
}