using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Guilder.Server.Authentication;
using Guilder.Server.Connectors;
using Guilder.Server.Connectors.Fake;
using Guilder.Server.Connectors.Graph;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Guilder.Server;

public static class DependencyRegistrations
{
    public static void AddGuilderOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AzureAppOptions>()
            .Bind(configuration.GetSection(AzureAppOptions.SectionName))
            .ValidateDataAnnotations();
    }

    public static void AddGuilderConfiguration<T>(this T configuration, bool isDevelopment)
        where T : IConfigurationBuilder, IConfiguration
    {
        try
        {
            configuration.AddAzureKeyVault(
                // Authenticating against it will be handled via AAD permissions.
                new Uri(configuration["KeyVaultUri"] ?? ""),
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
            if (isDevelopment)
            {
                const string configKey = $"{AzureAppOptions.SectionName}:{nameof(AzureAppOptions.ClientSecret)}";
                if (string.IsNullOrWhiteSpace(configuration[configKey]))
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
    }

    public static void AddMicrosoftGraph(this IServiceCollection services)
    {
        services.AddScoped<IMeetingRoomConnector, GraphConnector>();

        services.AddScoped<TokenCredential>(provider =>
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
        services.AddScoped(provider =>
        {
            TokenCredential credential = provider.GetRequiredService<TokenCredential>();
            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            return new GraphServiceClient(credential, scopes);
        });
    }

    public static void AddFakeConnection(this IServiceCollection services)
    {
        services.AddSingleton<IMeetingRoomConnector, FakeCurrentMeetingConnector>();
    }
}
