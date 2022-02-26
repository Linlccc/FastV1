using Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Role Interface 仓储
    /// </summary>
    public class RoleInterfaceClient : BaseClient<RoleInterface>, IRoleInterfaceClient
    {
        public RoleInterfaceClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<bool> SetRoleInterfaces(long roleId, long[] interfaces)
        {
            await DeleteAsync(ri => ri.RoleId == roleId);
            List<RoleInterface> roleInterfaces = interfaces.ToList().Select(i => new RoleInterface() { RoleId = roleId, InterfaceId = i }).ToList();
            await InsertRangeAsync(roleInterfaces);
            return true;
        }
    }
}