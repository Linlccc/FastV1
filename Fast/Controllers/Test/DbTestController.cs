using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Controllers.Test
{
    /// <summary>
    /// 测试数据库操作
    /// </summary>
    [CustomRoute(ApiGroup.Test)]
    public class DbTestController : Controller
    {
        private readonly ILogger<DbTestController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IUserClient _userClient;
        private readonly IRoleClient _roleClient;
        private readonly ITimedTaskClient _tasksInfoClient;
        private readonly IBaseClient<Role> _baseRole;

        public DbTestController(ILogger<DbTestController> logger, IUnitOfWork unitOfWork, ISqlSugarClient sqlSugarClient, IUserClient userClient, IRoleClient roleClient, ITimedTaskClient tasksInfoClient, IBaseClient<Role> baseRole)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _sqlSugarClient = sqlSugarClient;
            _userClient = userClient;
            _roleClient = roleClient;
            _tasksInfoClient = tasksInfoClient;
            _baseRole = baseRole;
        }

        /// <summary>
        /// 测试添加数据
        /// </summary>
        /// <returns></returns>
        public async Task AddData()
        {
            User u = new User()
            {
                LoginName = "Test1",
                LoginPWD = "123456",
                NickName = "T1"
            };
            await _userClient.InsertAsync(u);
            Role role = new Role()
            {
                Description = "123",
                Enabled = true,
                Name = "123"
            };
            var a = await _roleClient.InsertAsync(role);
        }

        /// <summary>
        /// 测试修改数据
        /// </summary>
        /// <returns></returns>
        public async Task UpdateData()
        {
            await _userClient.UpdateAsync(u => new User { NickName = "Tt1" }, u => u.LoginName == "Test1");
        }

        /// <summary>
        /// 测试修改数据
        /// </summary>
        /// <returns></returns>
        public async Task UpdateClomnData()
        {
            User u = new User()
            {
                Id = 154147154972741,
                LoginName = "Test11",
                LoginPWD = "112233",
                NickName = "T11"
            };
            await _userClient.UpdateColumnsAsync(u, u => new { u.LoginPWD, u.NickName });
        }

        /// <summary>
        /// 测试删除数据
        /// </summary>
        /// <returns></returns>
        public async Task DeleteData()
        {
            await _userClient.DeleteAsync(u => u.NickName == "T11");
        }

        public void TestUnit()
        {
            _unitOfWork.BeginTran();

            User u = _userClient.Single(u => u.Enabled);
            u.NickName = "事务内修改";
            var a = _userClient.Update(u);
            //执行另一个方法，这个方法也会开启事务，看这个方法提交事务会直接修改数据库吗
            TestUnit1();

            _unitOfWork.CommitTran();
        }

        private void TestUnit1()
        {
            _unitOfWork.BeginTran();

            Role role = _roleClient.Single(r => r.Enabled);
            role.Name = "事务内修改角色名";
            var a = _roleClient.Update(role);

            _unitOfWork.CommitTran();
        }

        public async Task<JsonResult> GetTest()
        {
            List<User> users = await _userClient.QueryAsync();
            List<TimedTask> tasks = await _tasksInfoClient.QueryAsync();

            return Json(new { users, tasks });
        }

        public async Task<long> GetMaxId()
        {
            return await _userClient.MaxFieldAsync(u => u.Id);
        }

        public async Task<List<Role>> GetRolesUseUserEntity()
        {
            return await _userClient.EntityDb<Role>().QueryAsync();
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _baseRole.QueryAsync();
        }

        public void TestYesNoOneU()
        {
            //_userClient.UnitOfWork.TranCount++;
            //_roleClient.UnitOfWork.TranCount++;
            //var a = _userClient.EntityDb<Role>();
            //a.UnitOfWork.TranCount++;

            //_userClient.UnitOfWork.TranCount++;
            //_roleClient.UnitOfWork.TranCount++;
        }
    }
}