using Attributes.EventBus;
using Autofac;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Extensions.ServiceExtensions.Autofacs
{
    /// <summary>
    /// 注入当前程序集中的类
    /// </summary>
    public class AutofacMainRegister : Module
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AutofacMainRegister));
        private readonly Type _type;

        /// <summary>
        /// 要注入控制器类的程序集中的类型
        /// </summary>
        /// <param name="type"></param>
        public AutofacMainRegister(Type type)
        {
            _type = type;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //注入控制器
            Type controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(_type.Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();

            //注入事件总线的处理程序类型,包含 SubscribeAttribute 特性
            builder.RegisterAssemblyTypes(this.GetType().Assembly)
                .Where(t => t.CheckAttribute<SubscribeAttribute>(false))
                .PropertiesAutowired();
        }
    }
}