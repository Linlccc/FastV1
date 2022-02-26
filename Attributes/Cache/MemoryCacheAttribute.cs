using System;

namespace Attributes.Cache
{
    /// <summary>
    /// 内存缓存特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MemoryCacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存有效时间（ms）
        /// 默认 30 分钟
        /// </summary>
        public int EffectiveTime { get; set; } = 1000 * 60 * 30;
    }
}