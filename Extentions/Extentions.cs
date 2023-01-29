namespace HostServer.Extentions
{
    public static class Extentions
    {
        public static string UnifySlash(this string str)
        {
            if(string.IsNullOrWhiteSpace(str))
                return str;
            str = str.Replace('\\', '/');
            if(str.Last() == '/')
                str = str.Remove(str.Length - 1, 1);
            return str;
        }

        public static string MergePath(this string path1, string path2)
        {
            path1 = path1.UnifySlash();
            path2 = path2.UnifySlash();

            if (string.IsNullOrWhiteSpace(path1))
                return path2;
            if (string.IsNullOrWhiteSpace(path2))
                return path1;

            if (path1.Last() == '/')
                path1 = path1.Remove(path1.Length -1 , 1);

            if (path2.StartsWith('/'))
                path2 = path2.Remove(0, 1);

            return $"{path1}/{path2}";
        }

    }
}
