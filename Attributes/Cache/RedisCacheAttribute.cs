using System;

namespace Attributes.Cache
{
    /// <summary>
    /// Redis 缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RedisCacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存有效时间（ms）
        /// 默认 30 分钟
        /// </summary>
        public int EffectiveTime { get; set; } = 1000 * 60 * 30;
    }
}