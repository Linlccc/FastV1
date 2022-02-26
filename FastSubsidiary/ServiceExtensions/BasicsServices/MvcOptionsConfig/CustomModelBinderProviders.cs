using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace FastSubsidiary.ServiceExtensions.BasicsServices
{
    /// <summary>
    /// 自定义模型绑定
    /// </summary>
    public class CustomModelBinderProviders : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            //string 处理,绑定值在body中的不做处理
            if (context.Metadata.ModelType == typeof(string) && context.Metadata.BindingSource?.DisplayName.ToLower() != "body")
                return new StringeModelBinder();

            return null;
        }

        /// <summary>
        /// string 类型模型绑定
        /// 为 null 时替换为 string.Empty
        /// </summary>
        public class StringeModelBinder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                string modelName = bindingContext.ModelName;
                if (modelName.IsNull()) modelName = bindingContext.OriginalModelName;
                //获取第一个值,如果为空替换为string.Empty
                string modelTypeValue = bindingContext.ValueProvider.GetValue(modelName).FirstValue ?? string.Empty;
                bindingContext.Result = ModelBindingResult.Success(modelTypeValue);
                return Task.CompletedTask;
            }
        }
    }
}