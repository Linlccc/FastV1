using Attributes.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Castle.DynamicProxy
{
    /// <summary>
    /// 面向切面的Redis使用
    /// </summary>
    public class RedisCacheAOP : CacheAOPBase
    {
        private readonly IRedisBasketRepository _cache;

        public RedisCacheAOP(IRedisBasketRepository cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        /// 如果有这个方法和相同参数的缓存值直接返回值
        /// </summary>
        /// <param name="invocation"></param>
        public override void Intercept(IInvocation invocation)
        {
            //直接代理的方法，也就是接口的方法声明
            MethodInfo interfaceMethod = invocation.Method ?? default;
            //目标类上面的方法，也就是执行的方法
            MethodInfo method = invocation?.MethodInvocationTarget ?? default;

            //是否 没有返回值
            bool isNotHavReturn = method.ReturnType == typeof(void) && method.ReturnType.IsNotResultAsync();
            //是否 没有启用redis缓存
            bool isNotEnableRedisCache = !interfaceMethod.CheckAttribute(false, out RedisCacheAttribute caching) && !method.CheckAttribute(false, out caching);
            //没有返回值 || 没有启用 redis 缓存
            if (isNotHavReturn || isNotEnableRedisCache)
            {
                invocation.Proceed();
                return;
            }

            //获取自定义缓存键
            string cacheKey = CustomCacheKey(invocation);
            //注意是 string 类型，方法GetValue
            string cacheValue = _cache.GetValue(cacheKey).Result;
            //如果不忽略的才直接返回缓存值
            if (cacheValue != null && !IgnoreCashe.CheckIsIgnore(cacheKey))
            {
                //将当前获取到的缓存值，赋值给当前执行方法
                Type returnType;
                if (method.ReturnType.IsHasResultAsync())//判断方法是否是异步的
                    returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();//从异步（task）中获取返回值类型
                else
                    returnType = method.ReturnType;//直接获取返回值类型

                dynamic _result = JsonConvert.DeserializeObject(cacheValue, returnType);//反序列化成指定类型的实例
                                                                                        //如果是异步的返回  Task<T> 类型，否则直接返回
                invocation.ReturnValue = method.ReturnType.IsHasResultAsync() ? Task.FromResult(_result) : _result;
                return;
            }
            //去执行当前的方法
            invocation.Proceed();

            //存入缓存
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                object response;

                //如果是返回异步类型
                if (invocation.ReturnValue.GetType().IsHasResultAsync()) response = ((dynamic)invocation.ReturnValue).Result;
                else response = invocation.ReturnValue;
                if (response == null) response = string.Empty;

                //保存缓存，过期时间30分钟
                _cache.Set(cacheKey, response, TimeSpan.FromMilliseconds(caching.EffectiveTime)).Wait();
            }
        }
    }
}