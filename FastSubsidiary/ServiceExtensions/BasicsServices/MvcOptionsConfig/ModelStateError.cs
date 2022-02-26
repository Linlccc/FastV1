using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.BasicsServices
{
    /// <summary>
    /// 模型验证错误自定义返回格式
    /// </summary>
    public static class ModelStateError
    {
        /// <summary>
        /// 模型验证错误自定义返回格式
        /// </summary>
        /// <param name="services"></param>
        public static void AddModelStateErrorSetup(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // 配置模型验证错误返回信息
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    List<string> msgs = actionContext.ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .Select(m => m.Value.Errors?.FirstOrDefault()?.ErrorMessage)
                    .ToList();

                    Msg<List<string>> result = MsgHelper.Fail(msgs.FirstOrDefault(), msgs);

                    BadRequestObjectResult objectResult = new(result)
                    {
                        StatusCode = result.Code
                    };
                    return objectResult;
                };
            });
        }
    }
}