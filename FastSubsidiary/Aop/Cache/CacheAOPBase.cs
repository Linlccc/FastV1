using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Castle.DynamicProxy
{
    public abstract class CacheAOPBase : IInterceptor
    {
        /// <summary>
        /// AOP的拦截方法
        /// </summary>
        /// <param name="invocation"></param>
        public abstract void Intercept(IInvocation invocation);

        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected static string CustomCacheKey(IInvocation invocation)
        {
            string typeName = invocation.TargetType.Name;  //被拦截的类型名
            string methodName = invocation.Method.Name;    //被拦截的方法名
            List<string> methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();//获取方法的参数列表，最多三个

            string key = $"{typeName}:{methodName}:";
            methodArguments.ForEach(m => key += $"{m}:");

            return key.TrimEnd(':');
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected static string GetArgumentValue(object arg)
        {
            if (arg is DateTime argDate) return argDate.ToString("yyyyMMddHHmmss");
            if (arg is string || arg is ValueType || arg is null) return arg.OToString();

            if (arg is Expression exp)
            {
                string result = Resolve(exp);
                return result.MD5Encrypt16();
            }
            if (arg.GetType().IsClass) return arg.Serialize().MD5Encrypt16();
            return string.Empty;
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected static string Resolve(Expression expression)
        {
            // lambda 表达式
            if (expression is LambdaExpression lambdaExp) return Resolve(lambdaExp.Body);
            // 二进制 表达式
            if (expression is BinaryExpression binaryExp)
            {
                // 解析x=>x.Name=="123" x.Age==123这类
                if (binaryExp.Left is MemberExpression leftMember && binaryExp.Right is ConstantExpression rightConstant) return ResolveFunc(leftMember, rightConstant, binaryExp.NodeType);
                //解析x=>x.Name.Contains("xxx")==false这类的
                if (binaryExp.Left is MethodCallExpression leftMethod && binaryExp.Right is ConstantExpression rightConstan) return ResolveLinqToObject(leftMethod, rightConstan.Value, binaryExp.NodeType);
                //解析x=>x.Name.Contains("xxx")==false这类的
                if (binaryExp.Left is MemberExpression leftMember1 && (binaryExp.Right is MemberExpression || binaryExp.Right is UnaryExpression))
                {
                    Delegate fn = Expression.Lambda(binaryExp.Right).Compile();
                    ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), binaryExp.Right.Type);
                    return ResolveFunc(leftMember1, value, binaryExp.NodeType);
                }
            }
            // 一元 表达式
            if (expression is UnaryExpression unaryExp)
            {
                //解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                if (unaryExp.Operand is MethodCallExpression operandMethod) return ResolveLinqToObject(operandMethod, false);
                //解析x=>!x.isDeletion这样的
                if (unaryExp.Operand is MemberExpression operandMember && unaryExp.NodeType == ExpressionType.Not) return ResolveFunc(operandMember, Expression.Constant(false), ExpressionType.Equal);
            }
            // 解析x=>x.isDeletion这样的
            if (expression is MemberExpression memberExp && expression.NodeType == ExpressionType.MemberAccess) return ResolveFunc(memberExp, Expression.Constant(true), ExpressionType.Equal);
            // x => x.Name.Contains("xxx")或array.Contains(x.Name)这类
            if (expression is MethodCallExpression methodCallExp) return ResolveLinqToObject(methodCallExp, true);

            BinaryExpression body = expression as BinaryExpression;
            if (body.IsNull()) return string.Empty;

            string @operator = GetOperator(body.NodeType);
            string left = Resolve(body.Left);
            string right = Resolve(body.Right);
            return $"({left} {@operator} {right})";
        }

        /// <summary>
        /// 获取操作类型
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private static string GetOperator(ExpressionType expressiontype)
        {
            return expressiontype switch
            {
                ExpressionType.And or ExpressionType.AndAlso => "and",
                ExpressionType.Or or ExpressionType.OrElse => "or",
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                _ => throw new Exception($"不支持 {expressiontype} 运算符查找！")
            };
        }

        /// <summary>
        /// 解析方法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private static string ResolveFunc(MemberExpression left, ConstantExpression right, ExpressionType expressiontype) => $"{left.Member.Name}{GetOperator(expressiontype)}{right.Value}" ?? "null";

        /// <summary>
        /// 解析 linq
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private static string ResolveLinqToObject(MethodCallExpression expression, object value, ExpressionType? expressiontype = null) => expression.Method.Name switch
        {
            "Contains" => expression.Object != null ? Like(expression) : In(expression, value),
            "Count" or "LongCount" => Len(expression, value, expressiontype.Value),
            _ => throw new Exception($"不支持 {expression.Method.Name} 方法的查找")
        };

        private static string In(MethodCallExpression expression, object isTrue)
        {
            ConstantExpression Argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
            MemberExpression Argument2 = expression.Arguments[1] as MemberExpression;
            FieldInfo Field_Array = Argument1.Value.GetType().GetFields().First();
            object[] Array = Field_Array.GetValue(Argument1.Value) as object[];
            List<string> SetInPara = new();
            foreach (object item in Array) SetInPara.Add(item.ToString());
            string Name = Argument2.Member.Name;
            string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
            string CompName = string.Join(",", SetInPara);
            string Result = $"{Name} {Operator} {CompName}";
            return Result;
        }

        private static string Like(MethodCallExpression expression)
        {
            ConstantExpression constant = Expression.Constant(Expression.Lambda(expression.Arguments[0]).Compile().DynamicInvoke(null), typeof(Expression));
            return $"{(expression.Object as MemberExpression).Member.Name} like %{constant}%";
        }

        private static string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            object Name = (expression.Arguments[0] as MemberExpression).Member.Name;
            string result = $"len({Name}){GetOperator(expressiontype)}{value}";
            return result;
        }
    }
}