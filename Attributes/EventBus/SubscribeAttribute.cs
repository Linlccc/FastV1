using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Attributes.EventBus
{
    /// <summary>
    /// 订阅事件总线
    /// 在类上说明要使用这个类订阅事件
    /// 放在方法上，使用这个方法处理该路由的事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SubscribeAttribute : Attribute
    {
        public SubscribeAttribute()
        {
        }

        /// <summary>
        /// 订阅事件总线
        /// </summary>
        /// <param name="routeKey">订阅的路由，也可以是你本次事件的操作关键词</param>
        public SubscribeAttribute(string routeKey)
        {
            this.RouteKey = routeKey;
        }

        /// <summary>
        /// 订阅的路由key
        /// </summary>
        public string RouteKey { get; set; }
    }

    /// <summary>
    /// 订阅特性的拓展
    /// </summary>
    public static class SubscribeExtension
    {
        /// <summary>
        /// 获取方法的订阅路由集合
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static List<string> GetRouteKeyByMethod(this MethodInfo methodInfo)
        {
            List<string> routes = new();
            List<SubscribeAttribute> subscribeAttributes = methodInfo.GetCustomAttributes(typeof(SubscribeAttribute), false).Select(a => a as SubscribeAttribute).ToList();//获取方法上的所有订阅特性
            routes = subscribeAttributes.Select(a => a.RouteKey).ToList();
            return routes;
        }
    }
}