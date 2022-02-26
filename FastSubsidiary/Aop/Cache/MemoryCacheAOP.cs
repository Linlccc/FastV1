using Attributes.Cache;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.Caching.Memory
{
    /// <summary>
    /// 面向切面的缓存使用
    /// </summary>
    public class MemoryCacheAOP : CacheAOPBase
    {
        private readonly ICachingProvider _cache;

        public MemoryCacheAOP(ICachingProvider cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
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
            //是否 没有启用内存缓存
            bool isNotEnableMemoryCache = !interfaceMethod.CheckAttribute(false, out MemoryCacheAttribute caching) && !method.CheckAttribute(false, out caching);
            //没有返回值 || 没有启用内存缓存
            if (isNotHavReturn || isNotEnableMemoryCache)
            {
                invocation.Proceed();
                return;
            }

            //获取自定义缓存键
            string cacheKey = CustomCacheKey(invocation);
            //根据key获取相应的缓存值
            object cacheValue = _cache.Get(cacheKey);
            //如果不忽略的才直接返回缓存值
            if (cacheValue != null && !IgnoreCashe.CheckIsIgnore(cacheKey))
            {
                invocation.ReturnValue = cacheValue;
                return;
            }
            //去执行当前的方法
            invocation.Proceed();
            //存入缓存
            if (cacheKey.IsNNull()) _cache.Set(cacheKey, invocation.ReturnValue, caching.EffectiveTime);
        }
    }
}