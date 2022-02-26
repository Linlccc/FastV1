using System;

namespace EventBus.EventBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public interface IEventBusr
    {
        /// <summary>
        /// 在一切准备就绪后调用，一般在完成订阅后执行
        /// 调用这个表示可以开始接收消息
        /// 避免程序启动就开始接收，部分订阅工作还未完成
        /// </summary>
        void Ready();

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="parameter">执行事件时使用的参数</param>
        void Publish(string routeKey, object parameter);

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="TH">事件处理类型</typeparam>
        /// <param name="routekey">参数类型名</param>
        void Subscribe<TH>(string routekey);

        void Subscribe(Type handlerType, string routekey);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="TH">事件处理类型</typeparam>
        /// <param name="routekey"></param>
        void UnSubscribe<TH>(string routekey);

        void UnSubscribe(Type handlerType, string routekey);
    }
}