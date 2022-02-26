using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.EventBusSubscriptions
{
    /// <summary>
    /// 事件总线订阅管理器
    /// 基于内存 单例模式
    /// </summary>
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 处理程序集合
        /// <处理程序类型,该处理程序处理的路由集合>
        /// </summary>
        private readonly Dictionary<Type, List<string>> _handlers = new Dictionary<Type, List<string>>();

        /// <summary>
        /// 事件订阅时触发
        /// 发出的数据（处理程序类型，路由）
        /// </summary>
        public event EventHandler<(Type, string)> OnSubscribe;

        /// <summary>
        /// 取消事件订阅时触发
        /// 发出的数据（处理程序类型，路由）
        /// </summary>
        public event EventHandler<(Type, string)> OnUnSubscribe;

        /// <summary>
        /// 是否为空
        /// 没有事件订阅
        /// </summary>
        public bool IsEmpty => !_handlers.Any();

        /// <summary>
        /// 清空订阅事件
        /// </summary>
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 是否已经订阅了该路由
        /// </summary>
        /// <param name="routeKey">路由</param>
        /// <returns></returns>
        public bool IsSubscribeByRouteKey(string routeKey) => _handlers.Values.Any(rs => rs.Contains(routeKey));

        /// <summary>
        /// 根据路由获取处理程序集合
        /// </summary>
        /// <param name="routeKey">路由</param>
        /// <returns>所有订阅这个路由的处理程序</returns>
        public List<Type> GetSubscriptionInfoModelsByRouteKey(string routeKey) => _handlers.Where(h => h.Value.Contains(routeKey)).Select(h => h.Key).ToList();

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <param name="TH">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        public void AddSubscription<TH>(string routekey) => AddSubscription(typeof(TH), routekey);

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <param name="handlerType">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        public void AddSubscription(Type handlerType, string routekey)
        {
            if (!_handlers.ContainsKey(handlerType)) _handlers.Add(handlerType, new List<string>());//如果没有该处理程序添加处理程序

            if (_handlers[handlerType].Any(r => r == routekey))
                throw new ArgumentException($"{handlerType.Name} 处理程序已经订阅 {routekey} 路由的信息");

            _handlers[handlerType].Add(routekey);//向该处理程序添加 routekey 路由的订阅

            OnSubscribe?.Invoke(this, (handlerType, routekey));//触发订阅事件
        }

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <param name="TH">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        public void RemoveSubscribe<TH>(string routekey) => RemoveSubscribe(typeof(TH), routekey);

        /// <summary>
        /// 移除订阅
        /// </summary>
        /// <param name="handlerType">处理程序类型</param>
        /// <param name="routekey">订阅路由</param>
        public void RemoveSubscribe(Type handlerType, string routekey)
        {
            if (!_handlers.ContainsKey(handlerType)) //判断是否订阅过这个处理程序
                throw new ArgumentException($"{handlerType.Name} 处理程序没有订阅过");

            _handlers[handlerType].Remove(routekey);//移除订阅
            //先判断是否还有其他 处理程序订阅这个路由，没有就移除路由订阅了
            if (!IsSubscribeByRouteKey(routekey))
                OnUnSubscribe?.Invoke(this, (handlerType, routekey));//触发移除订阅事件

            if (_handlers[handlerType].Any()) return;//如果该处理程序还有其他订阅直接返回，没有就移除该处理程序信息
            _handlers.Remove(handlerType);
        }
    }
}