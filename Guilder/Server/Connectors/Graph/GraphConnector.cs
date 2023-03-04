using Azure.Identity;
using Guilder.Server.Authentication;
using Guilder.Shared;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Guilder.Server.Connectors.Graph;

public class GraphConnector : IMeetingRoomConnector
{
    public IOptions<AzureAppOptions> AppOptions { get; set; }
    public GraphConnector(IOptions<AzureAppOptions> options)
    {
        AppOptions = options;
    }

    public async Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
    {
        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };
        var credential = new ClientSecretCredential(
            tenantId: AppOptions.Value.TenantId,
            clientId: AppOptions.Value.ClientId,
            clientSecret: AppOptions.Value.ClientSecret,
            options
        );
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var graphClient = new GraphServiceClient(credential, scopes);


        RoomCollectionResponse? rooms = await graphClient.Places.GraphRoom
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 10;
                });
        throw new NotImplementedException();
    }
}
