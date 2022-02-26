using System;
using System.Linq;
using System.Reflection;

namespace Attributes.Tool
{
    /// <summary>
    /// 备注特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class RemarkAttribute : Attribute
    {
        public RemarkAttribute(string remark)
        {
            Remark = remark;
        }

        public string Remark { get; set; }
    }

    public static class RemarkExtension
    {
        /// <summary>
        /// 获取类型的备注信息
        /// </summary>
        /// <param name="T">类型</param>
        /// <returns></returns>
        public static string GetTypeRemark<T>() => GetMemberRemark(typeof(T));

        /// <summary>
        /// 获取类型中成员的备注信息
        /// </summary>
        /// <param name="T">类型</param>
        /// <param name="propName">属性名称</param>
        /// <returns></returns>
        public static string GetMemberRemark<T>(string memberName)
        {
            Type type = typeof(T);
            MemberInfo member = type.GetField(memberName);
            member ??= type.GetProperty(memberName);
            return member.GetMemberRemark();
        }

        /// <summary>
        /// 获取成员的备注信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetMemberRemark(this MemberInfo memberInfo)
        {
            if (memberInfo != null && memberInfo.GetCustomAttributes(typeof(RemarkAttribute), false).FirstOrDefault() is RemarkAttribute remarkAttribute)
                return remarkAttribute.Remark;
            return string.Empty;
        }
    }
}