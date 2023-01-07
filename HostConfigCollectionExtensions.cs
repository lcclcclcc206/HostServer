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
    public static IApplicationBuilder UseHostStaticFile(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
    {
        var staticFileProviderList = configuration.GetSection("StaticFileProvider").GetChildren();

        foreach (var staticFileProvider in staticFileProviderList)
        {
            var provider = staticFileProvider.Get<StaticFileProvider>();
            if(Directory.Exists(provider!.RootPath))
            {
                app.RegisterStaticFile(provider!, logger);
                Configuration.StaticFileProviderList.Add(provider!);
            }
            else
            {
                logger.LogError($"The directory: {provider.RootPath} is not exist!");
                continue;
            }
            
        }

        return app;
    }

    private static IApplicationBuilder RegisterStaticFile(this IApplicationBuilder app, StaticFileProvider provider, ILogger logger)
    {
        try
        {
            var fileProvider = new PhysicalFileProvider(provider.RootPath!);
            var requestPath = provider.RequestPath!;

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath,
                ServeUnknownFileTypes = true,
                DefaultContentType = "octet-stream"
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
