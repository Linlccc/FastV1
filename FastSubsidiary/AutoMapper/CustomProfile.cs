using AutoMapper;
using Model.Models;
using Model.ViewModels;
using System.Linq;

namespace Extensions.AutoMapper
{
    public class CustomProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            //用户信息
            CreateMap<User, UserView>().AfterMap((u, v) =>
            {
                v.RoleNames = u.Roles?.Select(r => r.Name).ToList();
            });

            //角色
            CreateMap<Role, RoleView>();
            CreateMap<RoleView, Role>().AfterMap((rv, r) =>
            {
                r.Enabled = true;
            });
        }
    }
}