using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackExchange.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisBasketRepository
    {
        /// <summary>
        /// 获取 Reids 缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValue(string key);

        /// <summary>
        /// 获取值，并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> Get<TEntity>(string key);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        Task Set(string key, object value, TimeSpan cacheTime);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> Exist(string key);

        /// <summary>
        /// 移除某一个缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task Remove(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        /// <returns></returns>
        Task Clear();

        /// <summary>
        /// 根据key获取RedisValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<RedisValue[]> ListRangeAsync(string redisKey);

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        Task<long> ListLeftPushAsync(string redisKey, string redisValue, int db = -1);

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        Task<long> ListRightPushAsync(string redisKey, string redisValue, int db = -1);

        /// <summary>
        /// 在列表尾部插入数组集合。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        Task<long> ListRightPushAsync(string redisKey, IEnumerable<string> redisValue, int db = -1);

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素  反序列化
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<T> ListLeftPopAsync<T>(string redisKey, int db = -1) where T : class;

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素   反序列化
        /// 只能是对象集合
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<T> ListRightPopAsync<T>(string redisKey, int db = -1) where T : class;

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        Task<string> ListLeftPopAsync(string redisKey, int db = -1);

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        Task<string> ListRightPopAsync(string redisKey, int db = -1);

        /// <summary>
        /// 返回键处存储的列表的长度。 如果key不存在，则将其解释为空列表并返回0。
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        Task<long> ListLengthAsync(string redisKey, int db = -1);

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> ListRangeAsync(string redisKey, int db = -1);

        /// <summary>
        /// 根据索引获取指定位置数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start">开始位置，0是第一个元素</param>
        /// <param name="stop">结束位置，-1是最后一个</param>
        /// <param name="db"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> ListRangeAsync(string redisKey, int start, int stop, int db = -1);

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索,删除type个。小于零 : 从表尾开始向表头搜索,删除type个。等于零：移除表中所有与 VALUE 相等的值</param>
        /// <param name="db"></param>
        /// <returns></returns>
        Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1);

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        Task ListClearAsync(string redisKey, int db = -1);
    }
}