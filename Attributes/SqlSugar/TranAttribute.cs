using System;

namespace Attributes.SqlSugar
{
    /// <summary>
    /// SqlSugar 使用事务特性
    /// 建议放在接口方法声明上
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TranAttribute : Attribute
    {
    }
}