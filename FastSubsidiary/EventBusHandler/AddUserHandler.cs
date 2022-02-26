using Attributes.EventBus;
using Extensions.EventBusHandler.Models;
using Microsoft.Extensions.Logging;
using Model.Models;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Extensions.EventBusHandler
{
    /// <summary>
    /// 添加用户处理程序
    /// </summary>
    [Subscribe(null)]
    public class AddUserHandler
    {
        private readonly ILogger<AddUserHandler> _logger;
        private readonly IUserClient _userDb;

        public AddUserHandler(ILogger<AddUserHandler> logger, IUserClient userDb)
        {
            _logger = logger;
            _userDb = userDb;
        }

        [Subscribe("add:user")]
        public async Task AddUser(string routeKey, TestModel parameter)
        {
            Log(routeKey, parameter);

            await _userDb.InsertAsync(new User()
            {
                Id = parameter.id.OToInt(),
                LoginName = parameter.id,
                LoginPWD = "string",
                LastLoginTime = DateTime.Now,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                ErrorCount = 0,
                Enabled = true,
            });
        }

        [Subscribe("delete:Interface")]//这个只是测试在一个方法订阅两个路由
        [Subscribe("delete:user")]
        public async Task DeleteUser(string routeKey, dynamic parameter)
        {
            Log(routeKey, parameter);

            await _userDb.DeleteByIdAsync(parameter.id);
        }

        public void Log(string routeKey, object parameter)
        {
            _logger.LogInformation($"----- {this.GetType().Name} 处理 路由为 {routeKey} 的RabbitMQ的事件参数： - ({JsonConvert.SerializeObject(parameter)})");
            ConsoleHelper.WriteSuccessLine($"----- {this.GetType().Name} 处理 路由为 {routeKey} 的RabbitMQ的事件参数： - ({JsonConvert.SerializeObject(parameter)})");
        }
    }
}