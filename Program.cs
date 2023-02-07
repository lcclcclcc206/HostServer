using HostServer.Extensions;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using ByteSizeLib;
using HostServer.Models;
using Microsoft.Extensions.Logging;
using static HostServer.Models.StaticFileConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configuration/staticfile.json", optional: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Host.UseNLog();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = (long)ByteSize.FromGibiBytes(2).Bytes;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = (long)ByteSize.FromGibiBytes(2).Bytes;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodyBufferSize = (int)ByteSize.FromGibiBytes(2).Bytes;
});

var app = builder.Build();

ConfigUniversalFile(app.Configuration);
ConfigUploadFile(app.Configuration);
ConfigFileBrowser(app.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Logger.LogInformation("Host server start up");

app.Run();

static void ConfigUniversalFile(IConfiguration configuration)
{
    var universalFileConfigList = configuration.GetSection("StaticFile:Universal").GetChildren();

    foreach (var universalFileConfig in universalFileConfigList)
    {
        var config = universalFileConfig.Get<StaticFileConfig>();
        if (config is null)
        {
            continue;
        }
        else if (!Directory.Exists(config.RootPath))
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error($"The directory: {config.RootPath} is not exist!");
            continue;
        }
        else
        {
            HostConfiguration.StaticFileAccessDictionary.Add(config.AccessKey, config.RootPath);
            HostConfiguration.UniversalFileConfigList.Add(config);
        }
    }
}

static void ConfigUploadFile(IConfiguration configuration)
{
    var logger = LogManager.GetCurrentClassLogger();
    var uploadFileConfig = configuration.GetSection("StaticFile:UploadFile");
    var config = uploadFileConfig.Get<UploadFileConfig>();

    if (config is null)
    {
        logger.Warn($"Do not find the configuration of 'UploadFile' , use default configuration");
        config = HostConfiguration.UploadFileConfig;
    }

    if (!Directory.Exists(config.RootPath))
    {
        Directory.CreateDirectory(config.RootPath);
    }

    HostConfiguration.StaticFileAccessDictionary.Add(config.AccessKey, config.RootPath);
    HostConfiguration.UploadFileConfig = config;
}

static void ConfigFileBrowser(IConfiguration configuration)
{
    var section = configuration.GetSection("StaticFile:FileBrowser");
    var config = section.Get<FileBrowserConfig>();


    var logger = LogManager.GetCurrentClassLogger();
    if (config is null)
    {
        throw new NullReferenceException("The config of FileBrowser is not found!");
    }

    if (Directory.Exists(config.DefaultRootPath))
    {
        HostConfiguration.StaticFileAccessDictionary.Add(config.DefaultAccessKey, config.DefaultRootPath);
    }
    else
    {
        throw new NullReferenceException("The config of FileBrowser:DefaultRootPath is not found!");
    }

    HostConfiguration.FileBrowserConfig = config;
}