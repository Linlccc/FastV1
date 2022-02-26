using Microsoft.AspNetCore.Mvc;

namespace Fast.Controllers.Test
{
    /// <summary>
    /// Swagger 接口文档版本控制
    /// </summary>
    [CustomRoute(ApiGroup.Test)]
    public class ApiVersionController : Controller
    {
        private readonly SignalRController _signalRController;

        /// <summary>
        /// 测试使用注入的控制器
        /// </summary>
        /// <param name="signalRController"></param>
        public ApiVersionController(SignalRController signalRController)
        {
            _signalRController = signalRController;
        }

        public string GetVTestApi1()
        {
            _signalRController.AllSend("1231231231");
            return "1";
        }

        public string VTestApi2()
        {
            return "2";
        }
    }
}