using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 特性拓展
    /// </summary>
    public static class AttributeExtension
    {
        /// <summary>
        /// 获取指定类型特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="attributeProvider">特性提供者</param>
        /// <param name="inherit">是否从继承链上获取</param>
        /// <returns>指定类型的第一个特性</returns>
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit) where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
        }

        /// <summary>
        /// 验证是否包含指定特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="attributeProvider">特性提供者</param>
        /// <param name="inherit">是否从继承链上获取</param>
        /// <returns>包含该特性返回true</returns>
        public static bool CheckAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit) where TAttribute : Attribute
        {
            return attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() is TAttribute;
        }

        /// <summary>
        /// 验证是否包含指定特性
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="attributeProvider">特性提供者</param>
        /// <param name="inherit">是否从继承链上获取</param>
        /// <param name="attribute">指定特性的第一个值</param>
        /// <returns>包含该特性返回true</returns>
        public static bool CheckAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider, bool inherit, out TAttribute attribute) where TAttribute : Attribute
        {
            attribute = attributeProvider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
            return attribute != null;
        }
    }
}