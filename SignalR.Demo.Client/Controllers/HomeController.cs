using Microsoft.AspNetCore.Mvc;

namespace SignalR.Demo.Client.Controllers
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