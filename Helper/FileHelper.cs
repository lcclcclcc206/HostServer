namespace HostServer.Helper
{
    public class FileHelper
    {
        public static long GetDirectorySize(string path)
        {
            var options = new EnumerationOptions()
            {
                RecurseSubdirectories = true
            };
            var fileList = Directory.EnumerateFiles(path, "*", options);
            long sumSize = 0;
            foreach ( var file in fileList )
            {
                sumSize += new FileInfo(file).Length;
            }
            return sumSize;
        }
    }
}
