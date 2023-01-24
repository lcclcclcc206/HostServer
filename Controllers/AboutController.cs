using Microsoft.AspNetCore.Mvc;

namespace HostServer.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
