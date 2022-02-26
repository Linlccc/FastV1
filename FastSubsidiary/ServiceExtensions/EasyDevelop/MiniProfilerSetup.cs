using Microsoft.Extensions.DependencyInjection;

namespace Extensions.ServiceExtensions.EasyDevelop
{
    public static class MiniProfilerSetup
    {
        /// <summary>
        /// 性能检测
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProfilerSetup(this IServiceCollection services)
        {
            services.AddMiniProfiler(configureOptions =>
            {
                configureOptions.RouteBasePath = "/profiler";   //路由路径
                configureOptions.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;          //探测器显示按钮的位置
                configureOptions.PopupShowTimeWithChildren = true;       //决定默认情况下是否显示“时间”列
            });
        }
    }
}