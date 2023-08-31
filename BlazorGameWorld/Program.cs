using BlazorGameWorld;
using BlazorGameWorld.Data;
using BlazorSignalRApp.Server.Hubs;
using blazorWords.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add services to the container.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<IWordService, WordService>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

var wordHub = new HubConnectionBuilder()
	.WithUrl(app.Services.GetRequiredService<NavigationManager>().ToAbsoluteUri("/wordhub"))
	.Build();

await wordHub.StartAsync();

await app.RunAsync();
