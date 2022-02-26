namespace Microsoft.Extensions.Caching.Memory
{
    /// <summary>
    /// 简单的缓存接口，暂时只有查询和添加
    /// </summary>
    public interface ICachingProvider
    {
        /// <summary>
        /// 从缓存获取数据
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <returns>值</returns>
        object Get(string cacheKey);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="cacheValue">值</param>
        /// <param name="Expiration">过期时间（毫秒）</param>
        void Set(string cacheKey, object cacheValue, int Expiration);
    }
}