using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Guilder.Server.Authentication;
using Guilder.Shared;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware;
using System.Net;

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
        // URI to proxy
        //var proxyAddress = "http://localhost:8888";

        // Create a new System.Net.Http.HttpClientHandler with the proxy
        var handler = new HttpClientHandler
        {
            // Create a new System.Net.WebProxy
            // See WebProxy documentation for scenarios requiring
            // authentication to the proxy
            //Proxy = new WebProxy(new Uri(proxyAddress))
        };

        // Create an options object for the credential being used
        // For example, here we're using a ClientSecretCredential so
        // we create a ClientSecretCredentialOptions object
        var options = new ClientSecretCredentialOptions
        {
            // Create a new Azure.Core.HttpClientTransport
            Transport = new HttpClientTransport(handler)
        };

        var credential = new ClientSecretCredential(
            "37321907-14a5-4390-987d-ec0c66c655cd",
            "0afe78b9-c71f-473f-af41-68068728b215",
            "YOUR_CLIENT_SECRET",
            options
        );

        var scopes = new[] { "https://graph.microsoft.com/.default" };

        // This example works with Microsoft.Graph 5+
        var httpClient = GraphClientFactory.Create();

        var graphClient = new GraphServiceClient(httpClient, new AzureIdentityAuthenticationProvider(credential, scopes));

        var handlers = GraphClientFactory.CreateDefaultHandlers();

        // Remove a default handler
        //var compressionHandler =
        //    handlers.Where(h => h is CompressionHandler).FirstOrDefault();
        //handlers.Remove(compressionHandler);
        // ChaosHandler simulates random server failures
        ///handlers.Add(new ChaosHandler());
        
        var interactiveCredential = new InteractiveBrowserCredential();

        var authProvider = new AzureIdentityAuthenticationProvider(interactiveCredential, scopes);

        var customGraphClient = new GraphServiceClient(httpClient, authProvider);

        var messages = await graphClient.Me.Messages
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 100;
                    requestConfiguration.QueryParameters.Select = new string[] { "subject" };
                });

        throw new NotImplementedException();
    }
}
