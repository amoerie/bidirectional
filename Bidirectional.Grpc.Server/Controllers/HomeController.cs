using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Grpc.Server.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Home()
        {
            return View();
        }
    }
}