using FastTool.GlobalVar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Linq;

namespace FastSubsidiary.ServiceExtensions.BasicsServices
{
    /// <summary>
    /// Api 动作约定
    /// </summary>
    public class ApiControllerActionConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            //遍历所有控制器信息(给所有没有配置请求动作的api方法添加请求动作)
            foreach (ControllerModel controller in application.Controllers)
            {
                //方法信息
                foreach (ActionModel action in controller.Actions)
                {
                    #region 切割方法名
                    string[] works = action.ActionMethod.Name.SplitCamelCase();//骆驼命名式切割方法名
                    string work = works.First().ToLower();//切割后的第一个关键词
                    //方法名由多个部分组成 （GetList） && 请求方法字典包含该请求方式 && 不保留完全方法名
                    if (works.Length > 1 && ApiAction.VerbToHttpMethods.ContainsKey(work) && !ApiAction.IsKeepFullApiName)
                        action.ActionName = action.ActionMethod.Name.Replace(works[0], "");
                    #endregion 切割方法名

                    #region 配置请求动作绑定
                    SelectorModel selectorModel = action.Selectors[0];
                    //如果已经配置请求动作，跳过
                    if (selectorModel.ActionConstraints.Count > 0) continue;

                    //获取请求谓词
                    string verb = ApiAction.VerbToHttpMethods.ContainsKey(work) ? ApiAction.VerbToHttpMethods[work] : ApiAction.DefaultHttpMethod;

                    //添加请求方法
                    selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new string[] { verb }));
                    HttpMethodAttribute httpMethod = verb switch
                    {
                        "GET" => new HttpGetAttribute(),
                        "POST" => new HttpPostAttribute(),
                        "PUT" => new HttpPutAttribute(),
                        "DELETE" => new HttpDeleteAttribute(),
                        "PATCH" => new HttpPatchAttribute(),
                        "HEAD" => new HttpHeadAttribute(),
                        _ => throw new NotSupportedException(verb)
                    };
                    selectorModel.EndpointMetadata.Add(httpMethod);
                    #endregion 配置请求动作绑定

                    #region Post自动将值类型的参数设置 FromForm,这个不要，如果要从 form获取值，在参数上加特性，设置这个过后就不能重query获取参数了
                    ////是 Post 请求
                    //bool isPost = selectorModel.ActionConstraints.Any(a => a is HttpMethodActionConstraint hmac && hmac.HttpMethods.Any(h => h.ToLower() == "post"));
                    //// 没有设置数据绑定源的参数
                    //List<ParameterModel> Parameters = action.Parameters.Where(p=>!p.Attributes.Any(a=>a.GetType() == typeof(IBindingSourceMetadata))).ToList();
                    ////是post请求 && 有 没有设置绑定源的参数
                    //if (isPost && Parameters.Count > 0)
                    //{
                    //}
                    #endregion Post自动将值类型的参数设置 FromForm,这个不要，如果要从 form获取值，在参数上加特性，设置这个过后就不能重query获取参数了
                }
            }
        }
    }
}