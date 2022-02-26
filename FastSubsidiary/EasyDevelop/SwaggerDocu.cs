using FastTool.GlobalVar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Linq;

namespace Extensions.Middlewares.EasyDevelop
{
    public static class SwaggerDocu
    {
        public static void InitSwagger(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            //使用Swagger（api文档）
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                typeof(ApiGroup).GetEnumNames().ToList().ForEach(version =>
                {
                    setupAction.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"接口版本--{version}");  //这个json文件来自Swagger服务
                });

                //接口和模型不展开
                setupAction.DocExpansion(DocExpansion.None);

                //模型拓展深度（-1不显示模型）
                //setupAction.DefaultModelsExpandDepth(-1);

                //这个是性能检测的
                if (MiddlewareInfo.MiniProfiler)
                    setupAction.HeadContent = "<script async id='mini-profiler' src='/profiler/includes.min.js?v=4.2.1+b27bea37e9' data-version='4.2.1+b27bea37e9' data-path='/profiler/' data-current-id='03a96ffa-eeb0-4d50-abe1-bdc1a8843a6f' data-ids='03a96ffa-eeb0-4d50-abe1-bdc1a8843a6f' data-position='Left'' data-scheme='Light' data-authorized='true' data-children='true' data-max-traces='5' data-toggle-shortcut='Alt+P' data-trivial-milliseconds='2.0' data-ignored-duplicate-execute-types='Open,OpenAsync,Close,CloseAsync'></script>";

                // 路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写RoutePrefix = "doc";
                setupAction.RoutePrefix = "";
            });
        }
    }
}