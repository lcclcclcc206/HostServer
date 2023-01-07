namespace HostServer
{
    public static class Configuration
    {
        public static List<StaticFileProvider> StaticFileProviderList = new();
    }
    public class StaticFileProvider
    {
        public string? DisplayName { get; set; }
        public string? RootPath { get; set; }
        public string? RequestPath { get; set; }
    }
}
