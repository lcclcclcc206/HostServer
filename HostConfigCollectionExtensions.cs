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
        var universalFileProviderList = configuration.GetSection("Universal").GetChildren();

        foreach (var universalFileProvider in universalFileProviderList)
        {
            var provider = universalFileProvider.Get<StaticFileProvider>();
            if (Directory.Exists(provider!.RootPath))
            {
                app.RegisterStaticFile(provider!, logger);
                HostConfiguration.UniversalFileProviderList!.Add(provider!);
            }
            else
            {
                logger.LogError($"The directory: {provider.RootPath} is not exist!");
                continue;
            }
        }
        return app;
    }

    public static IApplicationBuilder UseUploadFile(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
    {
        var uploadFileProvider = configuration.GetSection("UploadFile");
        var provider = uploadFileProvider.Get<UploadFileStaticFileProvider>();

        string defaultRootPath = "./static/uploadfile";
        string defaultRequestPath = "/static/uploadfile";

        if (provider is null)
        {
            logger.LogWarning($"Do not find the configuration of 'UploadFile' , use default configuration, RootPath: {defaultRootPath}, RequestPath: {defaultRequestPath}");
            provider = new UploadFileStaticFileProvider
            {
                RootPath = defaultRootPath,
                RequestPath = defaultRequestPath
            };
        }

        if (Path.Exists(provider.RootPath) is false)
        {
            if (provider.RootPath is null)
            {
                provider.RootPath = defaultRootPath;
                logger.LogWarning($"Do not find the configuration of 'UploadFile:RootPath' , use default configuration, RootPath: {defaultRootPath}");
            }
            else
            {
                logger.LogWarning($"The configuration of 'UploadFile:RootPath' is not Exist, create the directory: {provider.RootPath}");
                Directory.CreateDirectory(provider.RootPath!);
            }
        }

        app.RegisterStaticFile(provider, logger);
        HostConfiguration.UploadFileProvider = provider;

        return app;
    }
    private static IApplicationBuilder RegisterStaticFile(this IApplicationBuilder app, StaticFileProvider provider, ILogger logger)
    {
        try
        {
            provider.RootPath = Path.GetFullPath(provider.RootPath!);
            var fileProvider = new PhysicalFileProvider(provider.RootPath);
            var requestPath = provider.RequestPath!;

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
            logger.LogError($"Can not find the directory: {provider.RootPath}\ninfo: {ex}");
        }

        return app;
    }
}
