using ByteSizeLib;
using HostServer.Extentions;
using HostServer.Helper;
using HostServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Web;

namespace HostServer.Controllers;

public class FileBrowserController : Controller
{
    [BindProperty]
    public string BasePath { get; set; } = @"D:/";

    private readonly ILogger _logger;

    public FileBrowserController(ILogger<FileUploadController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index(string? relativePath)
    {

        FileBrowserUnit browser = new()
        {
            BasePath = PathHelper.UnifySlash(Path.GetFullPath(BasePath)),
            RelativePath = relativePath is null ? "" : PathHelper.UnifySlash(relativePath)
        };

        var path = PathHelper.UnifySlashForWindows(browser.Path, true);
        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);

        foreach (var dir in dirs)
        {
            if (new FileInfo(dir).Attributes.HasFlag(FileAttributes.Hidden) is true)
                continue;

            var directoryUnit = new DirectoryUnit
            {
                RelativePath = Path.GetFileName(dir),
                ModifyTime = Directory.GetLastWriteTime(dir)
            };
            browser.DirectoryUnits.Add(directoryUnit);
        }

        foreach (var file in files)
        {
            var fileUnit = new FileUnit
            {
                Name = Path.GetFileName(file),
                ModifyTime = Directory.GetLastAccessTime(file),
                Size = ByteSize.FromBytes(new FileInfo(file).Length)
            };
            browser.FileUnits.Add(fileUnit);
        }

        return View(browser);
    }

    [HttpGet]
    public IActionResult DownloadFile(string fileName)
    {
        var path = PathHelper.UnifySlashForWindows(PathHelper.MergePath(BasePath, fileName), false);

        var stream = System.IO.File.OpenRead(path);
        var provider = new FileExtensionContentTypeProvider();
        string contentType;
        provider.TryGetContentType(path, out contentType!);
        return File(stream, contentType ?? "application/octet-stream", fileName);
    }
}
