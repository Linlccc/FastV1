using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StackExchange.Redis
{
    /// <summary>
    /// redis 操作类
    /// </summary>
    public class RedisBasketRepository : IRedisBasketRepository
    {
        private readonly ILogger<RedisBasketRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisBasketRepository(ILogger<RedisBasketRepository> logger, ConnectionMultiplexer redis)
        {
            this._logger = logger;
            this._redis = redis;
            this._database = redis.GetDatabase();
        }

        /// <summary>
        /// 获取第一个服务
        /// </summary>
        /// <returns></returns>
        private IServer GetServer()
        {
            EndPoint[] endPoints = _redis.GetEndPoints();
            return _redis.GetServer(endPoints.First());
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            //循环遍历删除每一个服务里面的key
            foreach (var endPoint in _redis.GetEndPoints())
            {
                foreach (var key in GetServer().Keys())
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }

        /// <summary>
        /// 是否存在这个键
        /// </summary>
        /// <param name="key"></param>
        /// <returns>存在返回true，否则false</returns>
        public async Task<bool> Exist(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        /// <summary>
        /// 获取指定 key 的对象值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns>成功返回对象，否则返回默认值</returns>
        public async Task<TEntity> Get<TEntity>(string key)
        {
            RedisValue value = await GetValue(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeExtension.DeSerializeFromByte<TEntity>(value);
            }
            else
            {
                //返回默认
                return default;
            }
        }

        /// <summary>
        /// 获取 string 值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetValue(string key)
        {
            return await _database.StringGetAsync(key);
        }

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        public async Task ListClearAsync(string redisKey, int db = -1)
        {
            //修剪指定列表的值
            await _database.ListTrimAsync(redisKey, 1, 0);
        }

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索,删除type个。小于零 : 从表尾开始向表头搜索,删除type个。等于零：移除表中所有与 VALUE 相等的值</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1)
        {
            return await _database.ListRemoveAsync(redisKey, redisValue, type);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素  反序列化
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string redisKey, int db = -1) where T : class
        {
            return SerializeExtension.DeSerializeFromByte<T>(await _database.ListLeftPopAsync(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string redisKey, int db = -1)
        {
            return await _database.ListLeftPopAsync(redisKey);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue, int db = -1)
        {
            return await _database.ListLeftPushAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string redisKey, string redisValue, int db = -1)
        {
            return await _database.ListRightPushAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表尾部插入数组集合。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string redisKey, IEnumerable<string> redisValue, int db = -1)
        {
            var redislist = new List<RedisValue>();
            foreach (var item in redisValue)
            {
                redislist.Add(item);
            }
            return await _database.ListRightPushAsync(redisKey, redislist.ToArray());
        }

        /// <summary>
        /// 返回键处存储的列表的长度。 如果key不存在，则将其解释为空列表并返回0。
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string redisKey, int db = -1)
        {
            return await _database.ListLengthAsync(redisKey);
        }

        /// <summary>
        /// 根据key获取RedisValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> ListRangeAsync(string redisKey)
        {
            return await _database.ListRangeAsync(redisKey);
        }

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, int db = -1)
        {
            var result = await _database.ListRangeAsync(redisKey);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 根据索引获取指定位置数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start">开始位置，0是第一个元素</param>
        /// <param name="stop">结束位置，-1是最后一个</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, int start, int stop, int db = -1)
        {
            var result = await _database.ListRangeAsync(redisKey, start, stop);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素   反序列化
        /// 只能是对象集合
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string redisKey, int db = -1) where T : class
        {
            return SerializeExtension.DeSerializeFromByte<T>(await _database.ListRightPopAsync(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListRightPopAsync(string redisKey, int db = -1)
        {
            return await _database.ListRightPopAsync(redisKey);
        }

        /// <summary>
        /// 移除指定key，如果存在忽略
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task Remove(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 设置值，如果存在覆盖
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">过期时间</param>
        /// <returns></returns>
        public async Task Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                await _database.StringSetAsync(key, value.SerializeToByte(), cacheTime);
            }
        }
    }
}