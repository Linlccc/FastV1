using Attributes.SqlSugar;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 数据库事务
    /// </summary>
    public class TranAOP : IInterceptor
    {
        private readonly ILogger<TranAOP> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public TranAOP(ILogger<TranAOP> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 实例化IInterceptor唯一方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            //直接代理的方法，也就是接口的方法声明
            MethodInfo interfaceMethod = invocation.Method ?? default;
            //目标类上面的方法，也就是执行的方法
            MethodInfo method = invocation?.MethodInvocationTarget ?? default;
            //接口的方法声明上没有事务特性 || 执行的方法上没有事务特性
            if (!interfaceMethod.CheckAttribute<TranAttribute>(false) && !method.CheckAttribute<TranAttribute>(false))
            {
                invocation.Proceed();//直接执行被拦截方法
                return;
            }

            //开启事务
            try
            {
                _unitOfWork.BeginTran();

                invocation.Proceed();

                _unitOfWork.CommitTran();
            }
            catch (Exception ex)
            {
                string[] carryParameter = invocation.Arguments.Select(a => a.GetType().IsEntrustOrExpression() ? a.ToString() : JsonConvert.SerializeObject(a ?? "", _jsonSettings)).ToArray();

                string log = $"【当前服务对象】：{ invocation.TargetType.Name + Environment.NewLine}" +
                    $"【当前执行方法】：{ invocation.Method.Name + Environment.NewLine}" +
                    $"【携带的参数有】：{string.Join(", ", carryParameter) + Environment.NewLine} 执行事务回滚";
                _logger.LogError(ex, log);
                _unitOfWork.RollbackTran();//如果出错数据回滚
            }
        }
    }
}