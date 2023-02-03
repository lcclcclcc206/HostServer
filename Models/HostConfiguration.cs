using System.Collections;

namespace HostServer.Models;

public static class HostConfiguration
{
    public static Dictionary<string, string> StaticFileAccessDictionary { get; set; } = new();

    public static List<StaticFileConfig> UniversalFileConfigList = new();

    public static UploadFileConfig? UploadFileConfig;
    public static FileBrowserConfig FileBrowserConfig = new()
    {
        DefaultAccessKey = "FileBrowserDefault",
        DefaultRootPath = "./"
    };
}