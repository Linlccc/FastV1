using Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// UserRole 仓储类
    /// </summary>
    public class UserRoleClient : BaseClient<UserRole>, IUserRoleClient
    {
        public UserRoleClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<bool> SetUserRoles(long userId, long[] roleIds)
        {
            await DeleteAsync(ur => ur.UserId == userId);
            List<UserRole> userRoles = roleIds.ToList().Select(r => new UserRole() { UserId = userId, RoleId = r }).ToList();
            await InsertRangeAsync(userRoles);
            return true;
        }
    }
}