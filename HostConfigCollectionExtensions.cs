using HostServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace HostServer.Extensions;
public static class HostConfigCollectionExtensions
{
    public static IServiceCollection AddHostStaticFile(this IServiceCollection services)
    {
        services.AddDirectoryBrowser();
        return services;
    }
    public static IApplicationBuilder UseUniversalFile(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
    {
        var universalFileConfigList = configuration.GetSection("StaticFile:Universal").GetChildren();

        foreach (var universalFileConfig in universalFileConfigList)
        {
            var config = universalFileConfig.Get<StaticFileConfig>();
            if (Directory.Exists(config!.RootPath))
            {
                HostConfiguration.StaticFileAccessDictionary.Add(config.AccessKey, config.RootPath);
                app.RegisterStaticFile(config!, logger);
                HostConfiguration.UniversalFileConfigList!.Add(config!);
            }
            else
            {
                logger.LogError($"The directory: {config.RootPath} is not exist!");
                continue;
            }
        }
        return app;
    }

    public static IApplicationBuilder UseUploadFile(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
    {
        var uploadFileConfig = configuration.GetSection("StaticFile:UploadFile");
        var config = uploadFileConfig.Get<UploadFileConfig>();

        string defaultRootPath = "./static/uploadfile";
        string defaultRequestPath = "/static/uploadfile";

        if (config is null)
        {
            logger.LogWarning($"Do not find the configuration of 'UploadFile' , use default configuration, RootPath: {defaultRootPath}, RequestPath: {defaultRequestPath}");
            config = new UploadFileConfig
            {
                RootPath = defaultRootPath,
                RequestPath = defaultRequestPath
            };
        }

        if (Path.Exists(config.RootPath) is false)
        {
            if (config.RootPath is null)
            {
                config.RootPath = defaultRootPath;
                logger.LogWarning($"Do not find the configuration of 'UploadFile:RootPath' , use default configuration, RootPath: {defaultRootPath}");
            }
            else
            {
                logger.LogWarning($"The configuration of 'UploadFile:RootPath' is not Exist, create the directory: {config.RootPath}");
                Directory.CreateDirectory(config.RootPath!);
            }
        }

        HostConfiguration.StaticFileAccessDictionary.Add(config.AccessKey, config.RootPath);
        app.RegisterStaticFile(config, logger);
        HostConfiguration.UploadFileConfig = config;

        return app;
    }

    public static IApplicationBuilder UseFileBrowser(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
    {
        var section = configuration.GetSection("StaticFile:FileBrowser");
        var config = section.Get<FileBrowserConfig>();
        var defaltAccessKey = HostConfiguration.FileBrowserConfig.DefaultAccessKey;
        var defaultPath = HostConfiguration.FileBrowserConfig.DefaultRootPath;

        if (config is null)
        {
            logger.LogWarning("The config of FileBrowser is not found, use default config");
            config = HostConfiguration.FileBrowserConfig;
            HostConfiguration.StaticFileAccessDictionary.Add(config.DefaultAccessKey, config.DefaultRootPath);
            return app;
        }

        if (config.DefaultAccessKey is null)
        {
            config.DefaultAccessKey = defaltAccessKey;
        }

        if (Directory.Exists(config.DefaultRootPath))
        {
            HostConfiguration.StaticFileAccessDictionary.Add(config.DefaultAccessKey, config.DefaultRootPath);
        }
        else
        {
            logger.LogError($"The directory: {config.DefaultRootPath} is not exist! Use default path: {defaultPath}");
            config.DefaultRootPath = defaultPath;
            HostConfiguration.StaticFileAccessDictionary.Add(config.DefaultRootPath, defaultPath);
        }

        HostConfiguration.FileBrowserConfig = config;

        return app;
    }

    private static IApplicationBuilder RegisterStaticFile(this IApplicationBuilder app, StaticFileConfig config, ILogger logger)
    {
        try
        {
            config.RootPath = Path.GetFullPath(config.RootPath!);
            var fileProvider = new PhysicalFileProvider(config.RootPath);
            var requestPath = config.RequestPath!;

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath,
                ServeUnknownFileTypes = true,
                DefaultContentType = "octet-stream",
                OnPrepareResponse = context =>
                {
                    var headers = context.Context.Response.Headers;
                    var contentType = headers["Content-Type"];
                    contentType += "; charset=utf-8";
                    headers["Content-Type"] = contentType;
                }
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });
        }
        catch (DirectoryNotFoundException ex)
        {
            logger.LogError($"Can not find the directory: {config.RootPath}\ninfo: {ex}");
        }

        return app;
    }
}
