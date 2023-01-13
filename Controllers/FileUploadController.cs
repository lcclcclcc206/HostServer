using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Security.Principal;

namespace HostServer.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(IFormFile uploadFile)
        {
            string filePath = Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), "static\\FileUpload", uploadFile.FileName);
            using(var stream = System.IO.File.Create(filePath))
            {
                await uploadFile.CopyToAsync(stream);
            }
            return View();
        }
    }
}
