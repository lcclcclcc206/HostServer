using Microsoft.AspNetCore.Mvc;

namespace HostServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
