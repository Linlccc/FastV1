using FastTool.GlobalVar;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Extensions.ServiceExtensions.EasyDevelop
{
    public static class SwaggerSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(nameof(SwaggerSetup));

        /// <summary>
        /// api 文档
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            string basePath = AppContext.BaseDirectory;       //xml文档路径
            string apiName = AppConfig.GetNode("BasicConfig", "ApiName");     //api名称

            services.AddSwaggerGen(setupAction =>
            {
                #region 配置版本json文档
                //定义一个或多个由Swagger生成器创建的文档
                //参数1   唯一标识文档的名称（访问的相对路径：/swagger/{version}/swagger.json）
                //参数2   页面上的一些全局变量
                typeof(ApiGroup).GetEnumNames().ToList().ForEach(group =>
                {
                    setupAction.SwaggerDoc(group, new OpenApiInfo
                    {
                        Title = $"{apiName} 接口文档——{RuntimeInformation.FrameworkDescription}",
                        Description = $"{apiName} HTTP API ",
                        Version = "1.0.0",
                        //Contact = new OpenApiContact { Name = apiName, Email = "", Url = new Uri("") }, //联系
                        //License = new OpenApiLicense { Name = apiName + " 官方没有文档", Url = new Uri("") }    // 公开的API的许可证信息。
                    });
                });

                //setupAction.OrderActionsBy(o => o.RelativePath);//根据路径名排序
                #endregion 配置版本json文档

                #region 接口 模型等注释
                try
                {
                    //方法的注释
                    string appXmlPath = Path.Combine(basePath, "Fast.xml");
                    //默认的第二个参数是false，这个是controller的注释，记得修改（true才显示控制器的注释）
                    setupAction.IncludeXmlComments(appXmlPath, true);

                    //实体的注释
                    string mlodelXmlPath = Path.Combine(basePath, "Model.xml");
                    setupAction.IncludeXmlComments(mlodelXmlPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请生成需要的xml文件，可在项目属性中配置，请检查并拷贝。{Environment.NewLine + ex.Message}");
                }
                #endregion 接口 模型等注释

                #region 这几个在需要身份验证的时候有用
                // 开启加权小锁
                setupAction.OperationFilter<AddResponseHeadersFilter>();
                setupAction.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                setupAction.OperationFilter<SecurityRequirementsOperationFilter>();
                #endregion 这几个在需要身份验证的时候有用

                switch (Authentication_AuthorizationInfo.UseAuthenticationSchemeName)
                {
                    case Authentication_AuthorizationInfo.AuthenticationSchemeName.Ids4:
                        // 接入identityserver4
                        setupAction.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                // 因为是 api 项目，那肯定是前后端分离的，所以用的是Implicit模式
                                Implicit = new OpenApiOAuthFlow
                                {
                                    // 这里配置 identityServer 项目的域名
                                    AuthorizationUrl = new Uri($"https://ids.dotnetrun.com/connect/authorize"),
                                    // 这里配置是 scope 作用域，
                                    // 只需要填写 api资源 的id即可，
                                    // 不需要把 身份资源 的内容写上，比如openid
                                    Scopes = new Dictionary<string, string>()
                                    {
                                        //{"FastCore.api","ApiResource id"},
                                        {"sapi","Api Scope"},
                                        {"openid","identity Scope"},
                                        {"profile","identity Scope"},
                                        {"email","identity Scope"},
                                        {"address","identity Scope"},
                                        {"phone","identity Scope"},
                                        {"offline_access","Scope"},
                                    }
                                }
                            }
                        });
                        break;

                    case Authentication_AuthorizationInfo.AuthenticationSchemeName.Jwt:
                        // Jwt Bearer 认证，必须是 oauth2
                        setupAction.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                            Name = "Authorization",//jwt默认的参数名称
                            In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                            Type = SecuritySchemeType.ApiKey
                        });
                        break;

                    case Authentication_AuthorizationInfo.AuthenticationSchemeName.Custom:
                        //这个看自定义登录是怎么判断的
                        break;

                    default:
                        break;
                }
            });
        }
    }
}