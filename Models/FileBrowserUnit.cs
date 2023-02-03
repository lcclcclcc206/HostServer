using ByteSizeLib;
using HostServer.Extentions;
using HostServer.Helper;
using System.Collections;

namespace HostServer.Models
{
    public class FileBrowserUnit
    {
        public string? AccessKey { get; set; }
        public string? BasePath { get; set; }
        public string? RelativePath { get; set; }
        public string Path
        {
            get
            {
                
                if(BasePath is null || RelativePath is null)
                    return "";
                BasePath = PathHelper.UnifySlash(BasePath);
                RelativePath = PathHelper.UnifySlash(RelativePath);
                return PathHelper.MergePath(BasePath,RelativePath);
            }
        }
        public List<FileUnit> FileUnits { get; set; } = new();
        public List<DirectoryUnit> DirectoryUnits { get; set; } = new();
    }

    public class FileUnit
    {
        public string? Name { get; set; }
        public DateTime? ModifyTime { get; set; }
        public ByteSize? Size { get; set; }
    }

    public class DirectoryUnit
    {
        public string? RelativePath { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
