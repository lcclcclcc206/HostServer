using ByteSizeLib;

namespace HostServer.Models;
public class StaticFileConfig
{
    public string AccessKey { get; set; } = "";
    public string DisplayName { get; set; } = "Default";
    public string RootPath { get; set; } = "./";
    public string RequestPath { get; set; } = "/static";
}

public class UploadFileConfig : StaticFileConfig
{
    public long FileSizeLimit { get; set; } = (long)ByteSize.FromMebiBytes(500).Bytes;
}

public class FileBrowserConfig
{
    public string DefaultAccessKey { get; set; } = "";
    public string DefaultRootPath { get; set; } = "./";
}