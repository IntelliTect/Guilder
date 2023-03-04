using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Guilder.Server.Authentication;
using Guilder.Server.Connectors;
using Guilder.Server.Connectors.Fake;
using Guilder.Server.Connectors.Graph;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NodaTime;
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

        // Add services to the container.
        builder.Services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            });
        builder.Services.AddRazorPages();

        //builder.Services.AddScoped<IMeetingRoomConnector, GraphConnector>();
        builder.Services.AddSingleton<IMeetingRoomConnector, FakeCurrentMeetingConnector>();

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);

        builder.Services.AddScoped<TokenCredential>(provider =>
        {
            IOptions<AzureAppOptions> option = provider.GetRequiredService<IOptions<AzureAppOptions>>();

            return new ClientSecretCredential(
                tenantId: option.Value.TenantId,
                clientId: option.Value.ClientId,
                clientSecret: option.Value.ClientSecret,
                new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                }
            );
        });
        builder.Services.AddScoped(provider =>
        {
            TokenCredential credential = provider.GetRequiredService<TokenCredential>();
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            return new GraphServiceClient(credential, scopes);
        });

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        try
        {
            builder.Configuration.AddAzureKeyVault(
                // Authenticating against it will be handled via AAD permissions.
                new Uri(builder.Configuration["KeyVaultUri"] ?? ""),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                {
                    ExcludeVisualStudioCredential = true,
                    ExcludeInteractiveBrowserCredential = true,
                    ExcludeSharedTokenCacheCredential = true,
                    ExcludeEnvironmentCredential = true,
                    ExcludeVisualStudioCodeCredential = true
                }),
                new AzureKeyVaultConfigurationOptions()
                {
                    ReloadInterval = TimeSpan.FromMinutes(10)
                }
            );
        }
        catch (Azure.RequestFailedException ex)
        {
            // We are unable to access the KeyVault. We are allowing this because someone may have a local secrets.json file.
            // If this is development, give a decent error message if they don't have the secret stored locally.
            if (builder.Environment.IsDevelopment())
            {
                const string configKey = $"{AzureAppOptions.SectionName}:{nameof(AzureAppOptions.ClientSecret)}";
                if (string.IsNullOrWhiteSpace(builder.Configuration[configKey]))
                {
                    throw new InvalidOperationException(
                        $"{configKey} is not set. This app is configured to use a KeyVault. Please configure using https://intellitect.com/blog/key-vault-configuration-provider/. A local secrets.json file can be used as well.",
                        ex
                    );
                }
            }
            else
            {
                // If this is production or there is no way to access the secret, throw a more useful exception.
                throw new InvalidOperationException("The Azure KeyVault is not accessible", ex);
            }
        }

        builder.Services.AddOptions<AzureAppOptions>()
            .Bind(builder.Configuration.GetSection(AzureAppOptions.SectionName))
            .ValidateDataAnnotations();

        builder.Services.AddSwaggerGen();

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
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();


        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
