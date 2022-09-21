using AspNetBlazor_RssFeeder.Services;
using AspNetBlazor_RssFeeder.Services.Interfaces;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// settings service
builder.Services.AddSingleton<ISettingsService, SettingsService>(_ => SettingsService.CreateWithFile());
builder.Services.AddSingleton<IFeeder, Feeder>();
builder.Services.AddTransient<AutoUpdater>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
