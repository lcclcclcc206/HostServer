using Microsoft.Extensions.FileProviders;
using HostServer.Extensions;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using ByteSizeLib;
using HostServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configuration/staticfile.json", optional: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostStaticFile();
builder.Host.UseNLog();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = (int)ByteSize.FromGibiBytes(1).Bytes;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = (int)ByteSize.FromGibiBytes(1).Bytes;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodyBufferSize = (int)ByteSize.FromGibiBytes(1).Bytes;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
