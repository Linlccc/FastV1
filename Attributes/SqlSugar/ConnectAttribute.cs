using System;

namespace Attributes.SqlSugar
{
    /// <summary>
    /// SqlSugar 连接特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConnectAttribute : Attribute
    {
        public ConnectAttribute(string connid)
        {
            Connid = connid;
        }

        /// <summary>
        /// 操作该实体时使用的数据库连接id
        /// </summary>
        public string Connid { get; set; }
    }
}