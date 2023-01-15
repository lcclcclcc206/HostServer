namespace HostServer.Models;

public static class HostConfiguration
{
    public static List<StaticFileProvider> UniversalFileProviderList = new();

    public static UploadFileStaticFileProvider? UploadFileProvider;
}