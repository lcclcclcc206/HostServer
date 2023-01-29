using ByteSizeLib;
using HostServer.Extentions;
using HostServer.Models;
using Microsoft.AspNetCore.Mvc;

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
            BasePath = Path.GetFullPath(BasePath).UnifySlash(),
            RelativePath = relativePath is null ? "" : relativePath.UnifySlash()
        };

        var dirs = Directory.GetDirectories(BasePath);
        var files = Directory.GetFiles(BasePath);

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


}
