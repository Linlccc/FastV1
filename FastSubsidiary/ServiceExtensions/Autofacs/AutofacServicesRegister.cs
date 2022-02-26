using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using FastTool.GlobalVar;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Extensions.ServiceExtensions.Autofacs
{
    public class AutofacServicesRegister : Autofac.Module
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AutofacServicesRegister));

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            string basePath = AppContext.BaseDirectory;

            #region 带有接口层的服务注入
            string repositoryDllFile = Path.Combine(basePath, "SugarClient.dll");   //仓储层dll

            if (!File.Exists(repositoryDllFile))
            {
                string msg = "SugarClient.dll 丢失，因为项目解耦了，请重新生成仓储层，然后复制到启动项目里。";
                _log.Error(msg);
                throw new Exception(msg);
            }

            List<Type> cacheType = new();
            if (AopInfo.RedisCachingAOP)
            {
                builder.RegisterType<RedisCacheAOP>();//注册要通过反射创建的组件
                cacheType.Add(typeof(RedisCacheAOP));
            }
            if (AopInfo.MemoryCachingAOP)
            {
                builder.RegisterType<MemoryCacheAOP>();
                cacheType.Add(typeof(MemoryCacheAOP));
            }
            if (AopInfo.TranAOP)
            {
                builder.RegisterType<TranAOP>();
                cacheType.Add(typeof(TranAOP));
            }
            if (AopInfo.DbOperAOP)
            {
                builder.RegisterType<DbOperAOP>();
                cacheType.Add(typeof(DbOperAOP));
            }

            // 获取 Repository.dll 程序集服务，并注册
            Assembly assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository)  //注册这个程序集中的类型
                   .Where(a => !a.GetInterfaces().Any(i => i == typeof(IUnitOfWork)))  //不注入事务处理类，单独注入
                   .AsImplementedInterfaces()                   //将类型注册为已实现的所有接口
                   .InstancePerLifetimeScope()                  //使用作用域，生命周期注入
                   .EnableInterfaceInterceptors()               //在目标类型上启用接口拦截。 拦截器将通过类或接口上的Intercept属性确定，或与InterceptedBy（）调用一起添加。
                   .InterceptedBy(cacheType.ToArray());         //允许将拦截器服务的列表分配给注册。

            //单独注入 UnitOfWork 和仓储基层
            builder.RegisterType(assemblysRepository.GetTypes().Single(t => t.Name.StartsWith("UnitOfWork"))).As<IUnitOfWork>().InstancePerLifetimeScope();
            //builder.RegisterGeneric(assemblysRepository.GetTypes().Single(t => t.Name.StartsWith("BaseClient"))).As(typeof(IBaseClient<>)).InstancePerLifetimeScope();
            #endregion 带有接口层的服务注入

            #region 没有接口层的服务层注入

            //因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            //注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            //var assemblysServicesNoInterfaces = Assembly.Load("Blog.Core.Services");
            //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion 没有接口层的服务层注入

            #region 没有接口的单独类，启用class代理拦截

            //只能注入该类中的虚方法，且必须是public
            //这里仅仅是一个单独类无接口测试，不用过多追问
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(cacheType.ToArray());
            #endregion 没有接口的单独类，启用class代理拦截

            #region 单独注册一个含有接口的类，启用interface代理拦截

            //不用虚方法
            //builder.RegisterType<AopService>().As<IAopService>()
            //   .AsImplementedInterfaces()
            //   .EnableInterfaceInterceptors()
            //   .InterceptedBy(typeof(BlogCacheAOP));
            #endregion 单独注册一个含有接口的类，启用interface代理拦截
        }
    }
}