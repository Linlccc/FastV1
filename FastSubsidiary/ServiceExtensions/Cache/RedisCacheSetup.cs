using InitQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Extensions.ServiceExtensions.Cache
{
    public static class RedisCacheSetup
    {
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            //如果要使用redis 在run的时候会来连接redis服务器
            //注入redis连接配置
            services.AddSingleton(sp =>
            {
                string redisConfiguration = AppConfig.GetNode("Redis", "ConnectionString");//获取连接字符串
                ConfigurationOptions configuration = ConfigurationOptions.Parse(redisConfiguration, true);//从以逗号分割连接字符中分析配置，忽略未知的
                configuration.ResolveDns = true;//在连接之前通过dns解析

                return ConnectionMultiplexer.Connect(configuration);//创建一个新的ConnectionMultiplexer实例
            });

            //注入redis的使用类，后面使用redis都是操作这个
            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();

            //Redis 消息列队
            if (AppConfig.GetBoolNode("Redis", "RedisMq"))
            {
                /* 测试步骤
                 * 1.启动消息列队
                 * 2.向 redis 服务器存入订阅类使用的key和 list数据类型的值
                 * 比如 RedisSubscribe 类的key是 RedisMqKey.Loging
                 */
                services.AddInitQ(m =>
                {
                    //时间间隔
                    m.SuspendTime = 5000;
                    //redis服务器地址
                    m.ConnectionString = AppConfig.GetNode("Redis", "ConnectionString");
                    //对应的订阅者类，需要new一个实例对象，当然你也可以传参，比如日志对象
                    //m.ListSubscribe = new List<Type>() { typeof(RedisSubscribe), typeof(RedisSubscribe2) };
                    //是否在控制台显示日志
                    m.ShowLog = false;
                });
            }
        }
    }
}