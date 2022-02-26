using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Extensions.Filter
{
    /// <summary>
    /// 方法过滤器
    /// 在方法上使用这个特性，调用这个方法是会执行这个特性
    /// [TypeFilter(typeof(UseServiceDIAttribute), Arguments = new object[] { "方法" })]
    /// </summary>
    public class UseServiceDIAttribute : ActionFilterAttribute
    {
        protected readonly ILogger<UseServiceDIAttribute> _logger;
        private readonly string _name;

        public UseServiceDIAttribute(ILogger<UseServiceDIAttribute> logger, string Name = "132")
        {
            _logger = logger;
            _name = Name;
        }

        /// <summary>
        /// 在模型绑定完成后，在操作执行之前调用
        /// 顺序：1
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 在方法执行之后、返回结果之前调用
        /// 顺序：2
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // var a = 1;
            //var dd =await _blogArticleServices.Query();
            base.OnActionExecuted(context);
            DeleteSubscriptionFiles();
        }

        /// <summary>
        /// 结果执行前
        /// 顺序：3
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }

        /// <summary>
        /// 结果执行后
        /// 顺序：4
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }

        private void DeleteSubscriptionFiles()
        {
            try
            {
                // ...
            }
            catch (Exception e)
            {
                _logger.LogError(e, "错误删除订阅文件");
            }
        }
    }
}