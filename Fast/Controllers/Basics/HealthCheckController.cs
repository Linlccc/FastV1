using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Controllers.Basics
{
    [ApiController, CustomRoute(ApiGroup.Consul)]
    public class HealthCheckController : Controller
    {
        public IActionResult GetCheck() => Ok();

        [Authorize]
        public IActionResult GetCheck1() => Ok();
    }
}