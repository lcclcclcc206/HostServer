using HostServer.Extentions;

namespace HostServer.Helper;

public static class PathHelper
{
    public static string UnifySlash(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return path;
        path = path.Replace('\\', '/');
        if (path.Last() == '/')
            path = path.Remove(path.Length - 1, 1);
        return path;
    }

    public static string UnifySlashForWindows(string path, bool isDirectory)
    {
        if (string.IsNullOrWhiteSpace(path))
            return path;
        path = path.Replace('/', '\\');
        if (isDirectory && path.Last() != '\\')
            path = path.Insert(path.Length, "\\") ;
        return path;
    }

    public static string MergePath(string path1, string path2)
    {
        path1 = UnifySlash(path1);
        path2 = UnifySlash(path2);

        if (string.IsNullOrWhiteSpace(path1))
            return path2;
        if (string.IsNullOrWhiteSpace(path2))
            return path1;

        if (path1.Last() == '/')
            path1 = path1.Remove(path1.Length - 1, 1);

        if (path2.StartsWith('/'))
            path2 = path2.Remove(0, 1);

        return $"{path1}/{path2}";
    }
}
