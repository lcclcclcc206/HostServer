using ByteSizeLib;

namespace HostServer.Models;
public class StaticFileProvider
{
    public string DisplayName { get; set; } = "Default";
    public string? RootPath { get; set; }
    public string? RequestPath { get; set; }
}

public class UploadFileStaticFileProvider : StaticFileProvider
{
    public ByteSize FileSizeLimit { get; set; } = ByteSize.FromMebiBytes(500);
}
