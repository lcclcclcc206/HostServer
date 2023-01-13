using Microsoft.Extensions.FileProviders;
using HostServer.Extensions;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostStaticFile();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseHostStaticFile(app.Configuration, app.Logger);

var fileUploadProvider = new PhysicalFileProvider(Path.Combine(Path.GetFullPath(Environment.CurrentDirectory), "static\\FileUpload"));
var fileUploadRequestPath = "/static/FileUpload";

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileUploadProvider,
    RequestPath = fileUploadRequestPath,
    ServeUnknownFileTypes = true,
    DefaultContentType = "octet-stream"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileUploadProvider,
    RequestPath = fileUploadRequestPath
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
