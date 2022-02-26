using System;
using System.Collections.Generic;

namespace EventBus.EventBusSubscriptions
{
    /// <summary>
    /// 订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 事件订阅时触发
        /// 发出的数据（处理程序类型，路由）
        /// </summary>
        event EventHandler<(Type, string)> OnSubscribe;

        /// <summary>
        /// 取消事件订阅时触发
        /// 发出的数据（处理程序类型，路由）
        /// </summary>
        event EventHandler<(Type, string)> OnUnSubscribe;

        /// <summary>
        /// 订阅处理程序的参数为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 清空所有处理程序
        /// </summary>
        void Clear();

        /// <summary>
        /// 是否已经订阅了该路由
        /// </summary>
        /// <param name="routeKey">路由</param>
        /// <returns></returns>
        bool IsSubscribeByRouteKey(string routeKey);

        /// <summary>
        /// 根据路由获取处理程序集合
        /// </summary>
        /// <param name="routeKey">路由</param>
        /// <returns>所有订阅这个路由的处理程序</returns>
        List<Type> GetSubscriptionInfoModelsByRouteKey(string routeKey);

        /// <summary>
        /// 添加定阅
        /// </summary>
        /// <param name="TH">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        void AddSubscription<TH>(string routekey);

        void AddSubscription(Type handlerType, string routekey);

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <param name="TH">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        void RemoveSubscribe<TH>(string routekey);

        void RemoveSubscribe(Type handlerType, string routekey);
    }
}