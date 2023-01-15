using HostServer.Models;
using HostServer.ViewModels.FileUpload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;


namespace HostServer.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly ILogger _logger;

        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(List<IFormFile> uploadFiles)
        {
            if (uploadFiles is null || uploadFiles.Count == 0)
            {
                _logger.LogError("Upload file fail!");
                return Problem("Upload file fail!");
            }

            foreach (IFormFile uploadFile in uploadFiles)
            {
                if (uploadFile.Length > HostConfiguration.UploadFileProvider!.FileSizeLimit)
                {
                    var requestId = Activity.Current?.Id;
                    var message = $"The file '{uploadFile.FileName}' size {uploadFile.Length} is over the limit!";
                    _logger.LogError(message);
                    return View("Error", new ErrorViewModel() { RequestId = requestId, ErrorMessage = message });
                    //return Problem($"The file '{uploadFile.FileName}' size {uploadFile.Length} is over the limit!");
                }
                string path = HostConfiguration.UploadFileProvider!.RootPath!;
                string filePath = Path.Combine(path, uploadFile.FileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await uploadFile.CopyToAsync(stream);
                }
            }

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
