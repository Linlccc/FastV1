using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Extensions.ServiceExtensions.BasicsServices
{
    /// <summary>
    /// 路由前缀公约
    /// </summary>
    public class GlobalRoutePrefix : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            //添加路由前缀
            string routePrefix = AppConfig.GetNode("BasicConfig", "RoutePrefix");
            if (routePrefix.IsNull()) return;

            AttributeRouteModel _centralPrefix = new(new RouteAttribute(routePrefix));
            //为所有特性添加前缀
            application.Controllers.ToList().ForEach(c =>
            {
                c.Selectors[0].AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_centralPrefix, c.Selectors[0].AttributeRouteModel);
            });
        }
    }
}