using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Guilder.Server.Tests;

public class WebApplicationFactory : WebApplicationFactory<Program>
{
    public string? Connector { get; set; } = "Fake";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ConfigurationBuilder configBuilder = new();
        configBuilder.AddInMemoryCollection(
                    new Dictionary<string, string?>()
                    {
                        {"GuilderConnector", Connector }
                    }
                    );
        builder.UseConfiguration(configBuilder.Build());
        base.ConfigureWebHost(builder);
    }
}
