using Microsoft.AspNetCore.Mvc;

namespace SignalR.Demo.Server.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}