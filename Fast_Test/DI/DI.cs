using Autofac;
using Fast;
using Microsoft.Extensions.DependencyInjection;

namespace Fast_Test
{
    public class DI
    {
        /// <summary>
        /// 创建di容器，注入
        /// </summary>
        /// <returns></returns>
        public static IContainer DIContainer()
        {
            IServiceCollection services = new ServiceCollection().AddLogging();
            services.AddAutoMapper(typeof(Startup));

            ContainerBuilder builder = new();
            IContainer container = builder.Build();
            return container;
        }
    }
}