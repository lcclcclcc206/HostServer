using Microsoft.Extensions.FileProviders;
using HostServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHostStaticFile();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseHostStaticFile(app.Configuration);

app.UseRouting();

app.MapGet("/ÄãºÃ", () =>
{
    return "Hello";
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();
