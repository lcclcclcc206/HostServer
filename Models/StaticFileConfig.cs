using ByteSizeLib;

namespace HostServer.Models;
public class StaticFileConfig
{
    public string AccessKey
    {
        get
        {
            accessKey ??= Guid.NewGuid().ToString();
            return accessKey;
        }
        set
        {
            accessKey = value;
        }
    }
    private string? accessKey;
    public string RootPath
    {
        get
        {
            return rootPath ?? throw new NullReferenceException($"The {nameof(RootPath)} is null!");
        }
        set
        {
            rootPath = value;
        }
    }
    private string? rootPath;
}

public class UploadFileConfig : StaticFileConfig
{
    public long FileSizeLimit { get; set; } = (long)ByteSize.FromMegaBytes(500).Bytes;
}

public class FileBrowserConfig
{
    public string DefaultAccessKey
    {
        get
        {
            defaultAccessKey ??= Guid.NewGuid().ToString();
            return defaultAccessKey;
        }
        set
        {
            defaultAccessKey = value;
        }
    }
    private string? defaultAccessKey;

    public string DefaultRootPath
    {
        get
        {
            return defaultRootPath ?? throw new NullReferenceException($"The {nameof(DefaultRootPath)} is null!");
        }
        set => defaultRootPath = value;
    }
    private string? defaultRootPath;
}

