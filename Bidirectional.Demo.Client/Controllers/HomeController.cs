using Microsoft.AspNetCore.Mvc;

namespace Bidirectional.Demo.Client.Controllers
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