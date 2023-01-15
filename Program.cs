using Microsoft.Extensions.FileProviders;
using HostServer.Extensions;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configuration/staticfile.json", optional: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostStaticFile();
builder.Host.UseNLog();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodyBufferSize = 1073741824;
});

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

app.UseUniversalFile(app.Configuration, app.Logger);

app.UseUploadFile(app.Configuration, app.Logger);

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Logger.LogInformation("Host server start up");

app.Run();
