using System.Collections;
using static HostServer.Models.StaticFileConfig;

namespace HostServer.Models;

public static class HostConfiguration
{
    public static Dictionary<string, string> StaticFileAccessDictionary { get; set; } = new();

    public static List<StaticFileConfig> UniversalFileConfigList { get; set; } = new();

    public static UploadFileConfig UploadFileConfig { get; set; } = new()
    {
        AccessKey = "UploadFile",
        RootPath = "./static/uploadfile"
    };

    public static FileBrowserConfig FileBrowserConfig { get; set; } = new();
}