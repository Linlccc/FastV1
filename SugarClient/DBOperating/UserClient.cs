using Model.Models;
using System;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// UserInfo 仓储类
    /// </summary>
    public class UserClient : BaseClient<User>, IUserClient
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> QueryUserRolesById(long id)
        {
            User user = await SugarClient.Queryable<User>()
                .Mapper<User, Role, UserRole>(ur => ManyToMany.Config(ur.UserId, ur.RoleId))
                .SingleAsync(u => u.Id == id);
            return user;
        }

        public async Task<PageMsg<User>> PageList(string name, int pageIndex, int pageSize)
        {
            RefAsync<int> totalDataCount = 0;
            PageMsg<User> pageUser = new PageMsg<User>(pageIndex, pageSize);
            pageUser.Data = await SugarClient.Queryable<User>()
            .Mapper<User, Role, UserRole>(ur => ManyToMany.Config(ur.UserId, ur.RoleId))
            .Where(u => u.LoginName.Contains(name) || u.NickName.Contains(name))
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPageListAsync(pageIndex, pageSize, totalDataCount);
            pageUser.TotalDataCount = totalDataCount;
            pageUser.TotalPageCount = (int)Math.Ceiling(totalDataCount / (decimal)pageSize);
            return pageUser;
        }
    }
}