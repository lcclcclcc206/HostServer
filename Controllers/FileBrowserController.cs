using ByteSizeLib;
using HostServer.Extentions;
using HostServer.Helper;
using HostServer.Models;
using HostServer.ViewModels;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO.Compression;
using System.IO.Pipes;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace HostServer.Controllers;

public class FileBrowserController : Controller
{
    [FromQuery]
    public string? AccessKey { get; set; }

    private readonly ILogger _logger;

    public FileBrowserController(ILogger<FileUploadController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index(string? relativePath)
    {
        var basePath = "./";
        AccessKey = AccessKey ?? HostConfiguration.FileBrowserConfig.DefaultAccessKey;
        if (HostConfiguration.StaticFileAccessDictionary.ContainsKey(AccessKey))
            basePath = HostConfiguration.StaticFileAccessDictionary[AccessKey];
        else
            _logger.LogError($"FileBrowser error: access key {AccessKey} is not exist!");

        FileBrowserUnit browser = new()
        {
            AccessKey= AccessKey,
            BasePath = PathHelper.UnifySlash(Path.GetFullPath(basePath)),
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
    public async Task<IActionResult> DownloadFile(string fileRequestPath)
    {
        var basePath = "./";
        AccessKey = AccessKey ?? HostConfiguration.FileBrowserConfig.DefaultAccessKey;
        if (HostConfiguration.StaticFileAccessDictionary.ContainsKey(AccessKey))
            basePath = HostConfiguration.StaticFileAccessDictionary[AccessKey];
        else
            _logger.LogError($"FileBrowser error: access key {AccessKey} is not exist!");

        var path = PathHelper.UnifySlashForWindows(PathHelper.MergePath(basePath, fileRequestPath), false);

        var provider = new FileExtensionContentTypeProvider();
        string contentType;
        if (!provider.TryGetContentType(path, out contentType!))
        {
            contentType = "application/octet-stream";
        }
        var fileName = Path.GetFileName(path);
        fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
        Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");
        Response.Headers.Add("Content-Type", $"{contentType}; charset=utf-8");

        int bufferSize = 1024 * 1024;

        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (Response.Body)
            {
                long contentLength = fs.Length;
                Response.ContentLength = contentLength;

                byte[] buffer;
                long hasRead = 0;

                while (hasRead < contentLength)
                {
                    if(HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        break;
                    }

                    buffer = new byte[bufferSize];

                    int currentRead = await fs.ReadAsync(buffer,0 , bufferSize);

                    await Response.Body.WriteAsync(buffer, 0, currentRead);

                    hasRead += currentRead;
                }
            }
        }

        return Empty;
    }

    [HttpGet]
    public async Task<IActionResult> DownloadDirectory(string relativePath)
    {
        var basePath = "./";
        AccessKey = AccessKey ?? HostConfiguration.FileBrowserConfig.DefaultAccessKey;
        if (HostConfiguration.StaticFileAccessDictionary.ContainsKey(AccessKey))
            basePath = HostConfiguration.StaticFileAccessDictionary[AccessKey];
        else
            _logger.LogError($"FileBrowser error: access key {AccessKey} is not exist!");

        var path = PathHelper.UnifySlashForWindows(PathHelper.MergePath(basePath, relativePath), true);
        var directoryInfo = new DirectoryInfo(path);

        long size = FileHelper.GetDirectorySize(directoryInfo.FullName);
        long sizeLimit = (long)ByteSize.FromMegaBytes(500).Bytes;
        if (size > sizeLimit)
        {
            string message = $"The directory size: {ByteSize.FromBytes(size).ToString()} is out of limit: {ByteSize.FromBytes(sizeLimit).ToString()}";
            return View("Error", new ErrorViewModel() { RequestId = Activity.Current?.Id, ErrorMessage = message });
        }

        var options = new EnumerationOptions()
        {
            RecurseSubdirectories = true
        };
        var fileList = Directory.EnumerateFiles(directoryInfo.FullName, "*", options);

        //Response.Headers.Add("Content-Type", $"charset=utf-8");
        Response.Headers.Add("Content-Disposition", $"filename=\"{HttpUtility.UrlEncode(directoryInfo.Name, Encoding.UTF8)}.zip\"");
        using (var stream = HttpContext.Response.BodyWriter.AsStream())
        {
            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                foreach (var file in fileList)
                {
                    var fileInfo = new FileInfo(file);
                    var filePath = fileInfo.FullName.Replace(directoryInfo.FullName, "");
                    if(filePath.StartsWith(Path.DirectorySeparatorChar.ToString(),StringComparison.Ordinal))
                    {
                        filePath = filePath.Substring(1);
                    }

                    var zipArchiveEntry =  zipArchive.CreateEntry(filePath, CompressionLevel.NoCompression);

                    using(var entryStream = zipArchiveEntry.Open())
                    {
                        using(var toZipStream = fileInfo.OpenRead())
                        {
                            await toZipStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }
        }

        return Empty;
    }

}
