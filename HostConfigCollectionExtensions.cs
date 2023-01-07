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
    public static IApplicationBuilder UseHostStaticFile(this IApplicationBuilder app, IConfiguration configuration)
    {
        var staticFileProviderList = configuration.GetSection("StaticFileProvider").GetChildren();
        foreach (var staticFileProvider in staticFileProviderList)
        {
            var provider = staticFileProvider.Get<StaticFileProvider>();
            app.RegisterStaticFile(provider!);
            Configuration.StaticFileProviderList.Add(provider!);
        }
        return app;
    }

    private static IApplicationBuilder RegisterStaticFile(this IApplicationBuilder app, StaticFileProvider provider)
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

        return app;
    }
}
