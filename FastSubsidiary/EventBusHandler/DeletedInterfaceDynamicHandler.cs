using Attributes.EventBus;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Extensions.EventBusHandler
{
    /// <summary>
    /// 动态 删除接口处理程序
    /// </summary>
    [Subscribe(null)]
    public class DeletedInterfaceDynamicHandler
    {
        private readonly ILogger<DeletedInterfaceDynamicHandler> _logger;
        private readonly IInterfaceInfoClient _interfaceInfoDb;

        public DeletedInterfaceDynamicHandler(ILogger<DeletedInterfaceDynamicHandler> logger, IInterfaceInfoClient interfaceInfoDb)
        {
            _logger = logger;
            _interfaceInfoDb = interfaceInfoDb;
        }

        [Subscribe("delete:Interface")]
        public async Task DeleteI(string routeKey, dynamic parameter)
        {
            _logger.LogInformation($"----- {this.GetType().Name} 处理 路由为 {routeKey} 的RabbitMQ的事件参数： - ({parameter})");
            ConsoleHelper.WriteSuccessLine($"----- {this.GetType().Name} 处理 路由为 {routeKey} 的RabbitMQ的事件参数： - ({parameter})");

            await _interfaceInfoDb.DeleteByIdAsync(parameter.id.ToString());
        }
    }
}