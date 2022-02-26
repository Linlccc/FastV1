using Attributes.EventBus;
using Autofac;
using EventBus.EventBusSubscriptions;
using EventBus.PersistentConnection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.EventBus
{
    /// <summary>
    /// 基于 RabbitMQ 的事件总线
    /// </summary>
    public class RabbitMQEventBus : IEventBusr, IDisposable
    {
        private readonly ILogger<RabbitMQEventBus> _logger;//日志纪录对象
        private readonly ILifetimeScope _autofac;//autofac容器
        private IModel _consumerChannel;//通用AMQP（高级消息队列协议）模型
        private string _queueName;//列队名称
        private readonly int _retryCount;//重试次数

        private const string _exchangeName = "fastcore_event_bus";//消息处理时和RabbitMQ 的交流名

        /// <summary>
        /// RabbitMQ持久连接
        /// </summary>
        private readonly IRabbitMQPersistentConnection _persistentConnection;

        /// <summary>
        /// 事件总线 订阅管理器
        /// </summary>
        private readonly IEventBusSubscriptionsManager _subsManager;

        /// <summary>
        /// RabbitMQ事件总线
        /// </summary>
        /// <param name="logger">日志</param>
        /// <param name="persistentConnection">RabbitMQ持久连接</param>
        /// <param name="autofac">autofac容器</param>
        /// <param name="subsManager">事件总线订阅管理器</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="retryCount">重试次数</param>
        public RabbitMQEventBus(ILogger<RabbitMQEventBus> logger, IRabbitMQPersistentConnection persistentConnection, ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName, int retryCount)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _autofac = autofac;
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _subsManager.OnUnSubscribe += SubsManager_OnUnSubscribe;
            _subsManager.OnSubscribe += SubsManager_OnSubscribe;
            _queueName = queueName;
            _retryCount = retryCount;

            _consumerChannel = CreateConsumerChannel();//创建消费通道
        }

        #region 取消订阅，订阅事件

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="unSubscribeInfo">取消的订阅信息</param>
        private void SubsManager_OnUnSubscribe(object sender, (Type, string) unSubscribeInfo)
        {
            //查看 持久连接是否连接状态
            CheckConnect();
            _consumerChannel.QueueUnbind(_queueName, _exchangeName, unSubscribeInfo.Item2);// 解除队列绑定
            _logger.LogInformation($"取消了 {unSubscribeInfo.Item1} 处理程序 {unSubscribeInfo.Item2} 的订阅");

            //如果订阅事件为空，列队名为空
            if (!_subsManager.IsEmpty) return;
            _queueName = string.Empty;
            _consumerChannel.Close();// 关闭和 RabbitMQ 的连接
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="unSubscribeInfo">取消的订阅信息</param>
        private void SubsManager_OnSubscribe(object sender, (Type, string) unSubscribeInfo)
        {
            CheckConnect();
            _consumerChannel.QueueBind(_queueName, _exchangeName, unSubscribeInfo.Item2);//列队绑定
            _logger.LogInformation($"订阅了 {unSubscribeInfo.Item1} 处理程序 {unSubscribeInfo.Item2}");
        }

        #endregion 取消订阅，订阅事件

        #region 接口实现

        /// <summary>
        /// 在一切准备就绪后调用，一般在完成订阅后执行
        /// 调用这个表示可以开始接收消息
        /// 避免程序启动就开始接收，部分订阅工作还未完成
        /// </summary>
        public void Ready() => StartBasicConsume();

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="routeKey">发布路由</param>
        /// <param name="parameter">参数</param>
        public void Publish(string routeKey, object parameter)
        {
            CheckConnect();

            RetryPolicy policy = Policy.Handle<BrokerUnreachableException>().Or<SocketException>()//可以处理的异常类型
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, $"无法在 {time.TotalSeconds:n1}s ({ex.Message}) 之后发布事件： {routeKey}");
                });

            _logger.LogInformation($"正在创建RabbitMQ通道用来发布 {routeKey} 路由的事件，参数：（{parameter}）");

            using IModel channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(_exchangeName, "direct");//声明 exchange ，类型为 direct

            //将参数转成字节
            string parameterStr = JsonConvert.SerializeObject(parameter);
            byte[] parameterBody = Encoding.UTF8.GetBytes(parameterStr);

            policy.Execute(() =>
            {
                IBasicProperties properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // 持久的

                _logger.LogInformation($"将 {routeKey} 路由的事件发布到RabbitMQ");
                //发布事件
                channel.BasicPublish(_exchangeName, routeKey, true, properties, parameterBody);
            });
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="TH">处理程序</typeparam>
        /// <param name="routeKey">路由</param>
        public void Subscribe<TH>(string routeKey) => Subscribe(typeof(TH), routeKey);

        public void Subscribe(Type handlerType, string routeKey) => _subsManager.AddSubscription(handlerType, routeKey);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TH">处理程序</typeparam>
        /// <param name="routeKey">路由</param>
        public void UnSubscribe<TH>(string routeKey) => _subsManager.RemoveSubscribe<TH>(routeKey);

        public void UnSubscribe(Type handlerType, string routeKey) => _subsManager.RemoveSubscribe(handlerType, routeKey);

        public void Dispose()
        {
            if (_consumerChannel != null) _consumerChannel.Dispose();
            _subsManager.Clear();
        }

        #region 帮助方法

        /// <summary>
        /// 检查连接，如果连接断开尝试连接
        /// </summary>
        private void CheckConnect()
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
        }

        #endregion 帮助方法
        #endregion 接口实现

        #region 创建消费通道，基本消费，及事件处理

        /// <summary>
        /// 创建RabbitMQ消费通道
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            CheckConnect();

            _logger.LogTrace("创建RabbitMQ消费通道");
            IModel channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(_exchangeName, "direct");//声明交换--直接
            channel.QueueDeclare(_queueName, true, false, false, null);//声明列队

            //异常回调
            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "通道错误，重建RabbitMQ消费通道");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        private void StartBasicConsume()
        {
            _logger.LogInformation("开始接收RabbitMQ消息");

            if (_consumerChannel == null)
            {
                _logger.LogError("开始基本消费无法调用，（通讯模型）_consumerChannel == null");
                return;
            }

            //将模型属性设置为给定值的构造函数
            AsyncEventingBasicConsumer consumer = new(_consumerChannel);
            consumer.Received += Consumer_Received;//接收到要执行事件

            //启动一个基本的内容类使用者
            _consumerChannel.BasicConsume(_queueName, false, consumer);
        }

        /// <summary>
        /// 消费者接受到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            string parameterTypeName = eventArgs.RoutingKey;//执行的路由
            string message = Encoding.UTF8.GetString(eventArgs.Body.Span);//执行事件时的参数

            try
            {
                //假异常
                if (message.ToLowerInvariant().Contains("throw-fake-exception")) throw new InvalidOperationException($"请求伪造的异常: '{message}'");

                await ProcessEvent(parameterTypeName, message);//去执行事件
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"----- 处理消息时出错 '{message}'");
            }

            // 即使在例外情况下，我们也会将消息从队列中移除
            // 在现实世界的应用程序中，这应该通过死信交换（DLX）来处理
            // 有关详细信息，请参阅: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, false);//确认一个或多个已接收
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="routeKey">路由</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        private async Task ProcessEvent(string routeKey, string parameter)
        {
            if (!_subsManager.IsSubscribeByRouteKey(routeKey))
            {
                _logger.LogWarning($"没有订阅 {routeKey} 路由的RabbitMQ事件");
                return;
            }

            _logger.LogTrace($"处理使用 {routeKey} 路由的RabbitMQ事件");
            using ILifetimeScope scope = _autofac.BeginLifetimeScope();
            List<Type> handlerTypes = _subsManager.GetSubscriptionInfoModelsByRouteKey(routeKey);//得到所有注册这个路由的处理程序
            foreach (Type handlerType in handlerTypes)
            {
                object handler = scope.ResolveOptional(handlerType);//处理类型对象
                List<MethodInfo> methodInfos = handlerType.GetMethods().Where(m => m.GetRouteKeyByMethod().Contains(routeKey)).ToList();//得到处理程序订阅该路由的方法信息
                foreach (MemberInfo member in methodInfos)
                {
                    //得到参数
                    Type parameterType = handlerType.GetMethod(member.Name).GetParameters()[1].ParameterType;
                    object parameterData = JsonConvert.DeserializeObject(parameter, parameterType);
                    //执行
                    await Task.Yield();
                    try
                    {
                        await (Task)handlerType.GetMethod(member.Name).Invoke(handler, new object[] { routeKey, parameterData });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"使用 {handlerType} 处理类型，执行 {routeKey} 路由错误");
                    }
                }
            }
        }

        #endregion 创建消费通道，基本消费，及事件处理
    }
}