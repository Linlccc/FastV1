using System;

namespace Attributes.Tool
{
    /// <summary>
    /// 访问日志忽略特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessLogIgnoreAttribute : Attribute
    { }
}