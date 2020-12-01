using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Demo.Server.Controllers
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