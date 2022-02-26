using System.Collections.Generic;
using System.Security.Claims;

namespace Extensions.Auth
{
    /// <summary>
    /// 用户信息接口
    /// </summary>
    public interface IUserInfo
    {
        string Name { get; }
        string ID { get; }

        /// <summary>
        /// 是否已通过身份验证
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated();

        /// <summary>
        /// 获取所有的声明
        /// </summary>
        /// <returns></returns>
        IEnumerable<Claim> GetClaimsIdentity();

        /// <summary>
        /// 获取 指定类型的声明值
        /// </summary>
        /// <param name="ClaimType">声明类型</param>
        /// <returns></returns>
        List<string> GetClaimValueByType(string ClaimType);

        /// <summary>
        /// 获取完整token字符串
        /// </summary>
        /// <returns></returns>
        string GetToken();

        /// <summary>
        /// 从 token 中获取指定类型的信息
        /// </summary>
        /// <param name="ClaimType">声明类型</param>
        /// <returns></returns>
        List<string> GetUserInfoFromToken(string ClaimType);
    }
}