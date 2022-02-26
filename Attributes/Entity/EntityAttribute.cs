using System;

namespace Attributes.Entity
{
    /// <summary>
    /// 实体类标记
    /// 表示是数据库实体类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityAttribute : Attribute
    {
    }
}