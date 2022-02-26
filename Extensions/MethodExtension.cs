namespace System.Reflection
{
    /// <summary>
    /// 方法拓展
    /// </summary>
    public static class MethodExtension
    {
        /// <summary>
        /// 是否是异步方法
        /// 判断返回类型是否是 Task || （返回类型是泛型类型 && 类型定义是 Task<> ）
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <returns>是异步返回true</returns>
        public static bool IsAsync(this MethodInfo method) => method.ReturnType.IsAsync();
    }
}