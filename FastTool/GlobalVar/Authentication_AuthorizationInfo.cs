using static Microsoft.Extensions.Configuration.AppConfig;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// 认证和授权变量配置
    /// </summary>
    public static class Authentication_AuthorizationInfo
    {
        /// <summary>
        /// 自定义策略的 policy
        /// </summary>
        public const string Name = "Permission";

        /// <summary>
        /// 获取当前使用的是否认证方案名称
        /// </summary>
        public static AuthenticationSchemeName UseAuthenticationSchemeName
        {
            get
            {
                bool isUseIds4 = GetBoolNode("Certified", "IdentityServer4", "Enabled");//是否使用ids4
                if (isUseIds4) return AuthenticationSchemeName.Ids4;
                bool isUseJwt = GetBoolNode("Certified", "JWT", "Enabled");//是否使用jwt
                if (isUseJwt) return AuthenticationSchemeName.Jwt;
                return AuthenticationSchemeName.Custom;
            }
        }

        /// <summary>
        /// 身份认证方案
        /// </summary>
        public enum AuthenticationSchemeName
        {
            Ids4,
            Jwt,
            Custom
        }
    }
}