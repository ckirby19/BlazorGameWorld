using BlazorGameWorld.Data;
using BlazorGameWorld.Hubs;
using BlazorGameWorld.Pages;
using BlazorSignalRApp.Server.Hubs;
using blazorWords.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		new[] { "application/octet-stream" });
});
builder.Services.AddTransient<IWordService, WordService>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<BlazorGameWorld.Shared.ConnectFourGameState>();

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
app.MapHub<WordHub>("/wordhub");
app.MapHub<ConnectFourHub>("/connectfourhub");
app.MapFallbackToPage("/_Host");

app.Run();
