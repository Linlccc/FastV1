using EventBus.EventBus;
using Extensions.EventBusHandler;
using Extensions.EventBusHandler.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Controllers.Test
{
    /// <summary>
    /// 事件总线测试
    /// </summary>
    [CustomRoute(ApiGroup.Test)]
    public class EventBusController : Controller
    {
        /// <summary>
        /// 测试RabbitMQ事件总线
        /// 调用删除接口
        /// </summary>
        /// <param name="_eventBus"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void EventBusTryDeleteI([FromServices] IEventBusr _eventBus, string id = "1")
        {
            //发布消息
            _eventBus.Publish("delete:Interface", new { id });
        }

        /// <summary>
        /// 测试RabbitMQ事件总线
        /// 调用添加用户
        /// </summary>
        /// <param name="_eventBus"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void EventBusTryAddUser([FromServices] IEventBusr _eventBus, string id = "1")
        {
            TestModel testModel = new(id);
            //发布消息
            _eventBus.Publish("add:user", testModel);
        }

        /// <summary>
        /// 测试RabbitMQ事件总线
        /// 调用删除用户
        /// </summary>
        /// <param name="_eventBus"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void EventBusTryDeleteUser([FromServices] IEventBusr _eventBus, string id = "1")
        {
            TestModel testModel = new(id);
            //发布消息
            _eventBus.Publish("delete:user", testModel);
        }

        /// <summary>
        /// 测试RabbitMQ事件总线
        /// 取消订阅
        /// </summary>
        /// <returns></returns>
        public void UnSubscribe([FromServices] IEventBusr _eventBus)
        {
            _eventBus.UnSubscribe<AddUserHandler>("delete:Interface");
        }
    }
}