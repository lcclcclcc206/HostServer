using ByteSizeLib;
using HostServer.Extentions;

namespace HostServer.Models
{
    public class FileBrowserUnit
    {
        public string? BasePath { get; set; }
        public string? RelativePath { get; set; }
        public string? Path
        {
            get
            {
                if(BasePath is null || RelativePath is null)
                    return null;
                return BasePath.MergePath(RelativePath);
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
