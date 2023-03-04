using System.Net;
using System.Text.Json;
using Guilder.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Guilder.Server.Tests.Connectors;

public class GraphConnectorTests : IDisposable
{
    private WebApplicationFactory<Program> Factory { get; } = new();
    private JsonSerializerOptions Options { get; } = new JsonSerializerOptions(
        JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    public GraphConnectorTests()
    {
    }
    
    [Fact]
    public async Task CreateMeeting_NewMeeting_Success()
    {
        HttpClient client = Factory.CreateClient();

        
        string roomId = "1";
        Instant expectedStart = new LocalDateTime(2022, 3, 16, 10, 0).InUtc().ToInstant();
        Instant expectedEnd = new LocalDateTime(2022, 3, 16, 10, 30).InUtc().ToInstant();
        Meeting expected = new("1", expectedStart, expectedEnd);
        HttpResponseMessage result = await client.PostAsJsonAsync(
            $"room/{roomId}/meeting", expected, Options);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        Meeting? actual = await result.Content.ReadFromJsonAsync<Meeting>(Options);

        Assert.Equal(expected, actual);
    }

    public void Dispose()
    {
        Factory?.Dispose();
    }
}
