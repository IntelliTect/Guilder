using Guilder.Server.Connectors;
using Guilder.Server.Connectors.Fake;
using NodaTime.Serialization.SystemTextJson;

namespace Guilder.Server;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging
            .AddConsole()
            // Filter out Request Starting/Request Finished noise:
            /*.AddFilter<ConsoleLoggerProvider>("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning)*/;

        builder.Configuration.AddGuilderConfiguration(builder.Environment.IsDevelopment());

        // Add services to the container.
        builder.Services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            });
        builder.Services.AddRazorPages();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);
        builder.Services.AddGuilderOptions(builder.Configuration);

        SetupConnector(builder);


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(opts =>
        {
            opts.ConfigObject.TryItOutEnabled = true;
        });

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();


        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }

    private static void SetupConnector(WebApplicationBuilder builder)
    {
        string? connector = builder.Configuration["GuilderConnector"];
        switch (connector)
        {
            case "Fake":
                builder.Services.AddFakeConnection();
                break;
            case "MicrosoftGraph":
                builder.Services.AddMicrosoftGraph();
                break;
            default:
                throw new InvalidOperationException($"Unknown connector '{connector}'");
        }
    }
}
