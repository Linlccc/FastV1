using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace EventBus.PersistentConnection
{
    /// <summary>
    /// RabbitMQ 持久连接
    /// </summary>
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly object _asyncLock = new();//异步锁

        private readonly ILogger<RabbitMQPersistentConnection> _logger;
        private readonly IConnectionFactory _connectionFactory;//连接工厂
        private readonly int _retryCount;//重试次数
        private IConnection _connection;//AMQP连接的主接口
        private bool _disposed;

        /// <summary>
        /// RabbitMQ 持久连接
        /// </summary>
        /// <param name="logger">日志纪录对象</param>
        /// <param name="connectionFactory">连接工厂</param>
        /// <param name="retryCount">重试次数</param>
        public RabbitMQPersistentConnection(ILogger<RabbitMQPersistentConnection> logger, IConnectionFactory connectionFactory, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected
        {
            get => _connection != null && _connection.IsOpen && !_disposed;
        }

        public IModel CreateModel()
        {
            if (!IsConnected) throw new InvalidOperationException("没有RabbitMQ连接可用于执行此操作");
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ客户端正在尝试连接");

            lock (_asyncLock)
            {
                //可应用于同步委托的重试策略
                RetryPolicy retryPolicy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>() //可以处理的异常类型
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, $"RabbitMQ客户端在{time.TotalSeconds:n1}s（{ex.Message}）之后无法连接");
                    });

                //执行 创建AMQP连接 操作
                retryPolicy.Execute(() => _connection = _connectionFactory.CreateConnection());

                if (!IsConnected)
                {
                    _logger.LogCritical("致命错误：无法创建和打开RabbitMQ连接");
                    return false;
                }

                _connection.ConnectionShutdown += OnConnectionShutdown;//连接被关闭
                _connection.CallbackException += OnCallbackException;//连接出现异常
                _connection.ConnectionBlocked += OnConnectionBlocked;//连接被阻止
                _logger.LogInformation($"RabbitMQ客户机获得了到{_connection.Endpoint.HostName}的持久连接，并订阅了失败事件");
                return true;
            }
        }

        #region 连接出现问题--重新连接

        /// <summary>
        /// 连接被阻止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e) => WriteLogAndTryConnect("RabbitMQ连接已关闭!正在尝试重新连接...");

        /// <summary>
        /// 连接出现异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCallbackException(object sender, CallbackExceptionEventArgs e) => WriteLogAndTryConnect("RabbitMQ连接引发异常!正在尝试重新连接...");

        /// <summary>
        /// 连接被关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason) => WriteLogAndTryConnect("RabbitMQ连接被关闭!正在尝试重新连接...");

        /// <summary>
        /// 写日志，并重新连接
        /// </summary>
        /// <param name="log"></param>
        private void WriteLogAndTryConnect(string log)
        {
            if (_disposed) return;

            _logger.LogWarning(log);

            TryConnect();
        }

        #endregion 连接出现问题--重新连接
    }
}