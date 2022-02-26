using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 类型拓展
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 获取类型名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetTypeName(this Type type)
        {
            if (!type.IsGenericType) return type.Name;

            string genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            string typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            return typeName;
        }

        /// <summary>
        /// 判断类型是否是 委托 || lamda 表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEntrustOrExpression(this Type type) => type.IsSubclassOf(typeof(Expression)) || (type.IsGenericType && type.GetGenericTypeDefinition().IsSubclassOf(typeof(Delegate)));

        /// <summary>
        /// 是否是异步
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsAsync(this Type type) => type.IsNotResultAsync() || type.IsHasResultAsync();

        /// <summary>
        /// 是否是没有返回结果的异步
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNotResultAsync(this Type type) => type == typeof(Task);

        /// <summary>
        /// 是否是有返回结果的异步
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsHasResultAsync(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
    }
}