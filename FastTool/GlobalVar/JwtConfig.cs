using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Microsoft.Extensions.Configuration.AppConfig;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// 应用程序秘钥配置
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// 秘钥
        /// </summary>
        public static string JwtSecret { get; } = GetNode("Certified", "JWT", "Secret");

        /// <summary>
        /// 对称安全密钥
        /// </summary>
        public static SymmetricSecurityKey SecurityKey { get => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecret)); }

        /// <summary>
        /// 发行人
        /// </summary>
        public static string Issuer { get; } = GetNode("Certified", "JWT", "Issuer");

        /// <summary>
        /// 订阅人
        /// </summary>
        public static string Audience { get; } = GetNode("Certified", "JWT", "Audience");
    }
}