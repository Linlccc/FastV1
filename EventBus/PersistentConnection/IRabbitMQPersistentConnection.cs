using RabbitMQ.Client;
using System;

namespace EventBus.PersistentConnection
{
    /// <summary>
    /// RabbitMQ 持久连接
    /// </summary>
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试 连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建 通用AMQP（高级消息队列协议）模型
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}