using Model.ViewModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Extensions.Auth
{
    /// <summary>
    /// JWTToken生成类
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="claims">需要在登陆的时候配置</param>
        /// <param name="permissionRequirement">在startup中定义的参数</param>
        /// <returns></returns>
        public static TokenInfoView BuildJwtToken(Claim[] claims, PermissionRequirement permissionRequirement)
        {
            DateTime now = DateTime.Now;
            // 实例化JwtSecurityToken
            JwtSecurityToken jwt = new(
                issuer: permissionRequirement.Issuer,                           //发行人
                audience: permissionRequirement.Audience,                       //订阅
                claims: claims,                                                 //声明
                notBefore: now,                                                 //生效时间
                expires: now.Add(permissionRequirement.Expiration),             //过期时间(使用的是在claims里面的过期时间)
                signingCredentials: permissionRequirement.SigningCredentials    //秘钥签证
            );
            // 生成 Token
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //打包返回前台
            TokenInfoView responseJson = new()
            {
                Token = encodedJwt,
                Expires_in = permissionRequirement.Expiration.TotalSeconds,
                Token_type = "Bearer"
            };
            return responseJson;
        }

        /// <summary>
        /// 解析 返回 TokenModelJwt 对象
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModelJwt SerializeJwt(string jwtStr)
        {
            JwtSecurityToken jwtToken = new(jwtStr);
            TokenModelJwt tm = new()
            {
                Uid = jwtToken.Id.OToLong(),
            };
            return tm;
        }
    }

    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Uid { get; set; }
    }
}