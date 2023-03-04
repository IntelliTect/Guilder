using Guilder.Client;
using Guilder.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<MeetingClient>();
builder.Services.AddScoped<TimelineStore>();

builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton<ICurrentTimeZone>(new CurrentTimeZone(DateTimeZoneProviders.Tzdb["America/Los_Angeles"]));

await builder.Build().RunAsync();
