using ByteSizeLib;
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
                var requestId = Activity.Current?.Id;
                var message = "Upload file fail!";
                _logger.LogError(message);
                return View("Error", new ErrorViewModel() { RequestId = requestId, ErrorMessage = message });
            }

            long contentSize = 0;
            foreach (IFormFile uploadFile in uploadFiles)
            {
                contentSize += uploadFile.Length;
            }

            if (contentSize > HostConfiguration.UploadFileProvider!.FileSizeLimit)
            {
                var requestId = Activity.Current?.Id;
                var message = $"The file size: {ByteSize.FromBytes(contentSize)} is over the limit!";
                _logger.LogError(message);
                return View("Error", new ErrorViewModel() { RequestId = requestId, ErrorMessage = message });
            }

            foreach (IFormFile uploadFile in uploadFiles)
            {
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
