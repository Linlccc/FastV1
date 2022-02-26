using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Extensions.Auth
{
    public class UserInfo : IUserInfo
    {
        private IHttpContextAccessor _accessor;

        public string Name => GetName();
        public string ID => GetClaimValueByType("jti").FirstOrDefault();

        public UserInfo(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private string GetName()
        {
            if (IsAuthenticated())//如果是加权接口直接返回
                return _accessor.HttpContext.User.Identity.Name;
            else //如果不是加权的就自己拼接
                return GetUserInfoFromToken(ClaimTypes.Name).FirstOrDefault().OToString();
        }

        public IEnumerable<Claim> GetClaimsIdentity() => _accessor.HttpContext == null ? new List<Claim>() : _accessor.HttpContext.User.Claims;

        public List<string> GetClaimValueByType(string claimType) => GetClaimsIdentity().Where(c => c.Type == claimType).Select(c => c.Value).ToList();

        public string GetToken() => _accessor.HttpContext?.Request.Headers["Authorization"].OToString().Replace("Bearer ", "");

        public List<string> GetUserInfoFromToken(string ClaimType)
        {
            string token = GetToken();
            if (token.IsNull()) return new List<string>();
            JwtSecurityToken jwtToken = new(token);
            return jwtToken.Claims.Where(c => c.Type == ClaimType).Select(c => c.Value).ToList();
        }

        public bool IsAuthenticated() => _accessor.HttpContext == null ? false : _accessor.HttpContext.User.Identity.IsAuthenticated;
    }
}