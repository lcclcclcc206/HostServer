using ByteSizeLib;
using HostServer.Extentions;
using HostServer.Helper;
using System.Collections;

namespace HostServer.Models
{
    public class FileBrowserUnit
    {
        public FileBrowserUnit(string accessKey, string basePath, string relativePath)
        {
            AccessKey = accessKey;
            BasePath = basePath;
            RelativePath = relativePath;
        }

        public string AccessKey { get; init; }
        public string BasePath { get; init; }
        public string RelativePath { get; init; }

        public string Path
        {
            get
            {

                if (BasePath is null || RelativePath is null)
                    return "";
                var basePath = PathHelper.UnifySlash(BasePath);
                var relativePath = PathHelper.UnifySlash(RelativePath);
                return PathHelper.MergePath(basePath, relativePath);
            }
        }
        public List<FileUnit> FileUnits { get; init; } = new();
        public List<DirectoryUnit> DirectoryUnits { get; init; } = new();
    }

    public class FileUnit
    {
        public FileUnit(string name, DateTime modifyTime, ByteSize size)
        {
            Name = name;
            ModifyTime = modifyTime;
            Size = size;
        }
        public string Name { get;init; }
        public DateTime ModifyTime { get; init; }
        public ByteSize Size { get; init; }
    }

    public class DirectoryUnit
    {
        public DirectoryUnit(string name, DateTime modifyTime)
        {
            Name = name;
            ModifyTime = modifyTime;
        }

        public string Name { get; init; }
        public DateTime ModifyTime { get; init; }
    }
}
