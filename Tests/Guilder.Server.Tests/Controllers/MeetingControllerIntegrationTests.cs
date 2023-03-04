using Guilder.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
namespace Guilder.Server.Tests.Controllers;

public class MeetingControllerIntegrationTests : IDisposable
{
    (string Name, string Id) BattleOfWits = ("1. Battle of Wits", "3a02a800-1e8a-49ef-82f6-be60e1147fdd");
    
    private WebApplicationFactory<Program> Factory { get; } = new();

    [Fact]
    public async Task CreateMeeting_NewMeeting_Success()
    {
        MeetingClient client = new(Factory.CreateClient());


        Instant expectedStart = new LocalDateTime(2022, 3, 16, 10, 0).InUtc().ToInstant();
        Instant expectedEnd = new LocalDateTime(2022, 3, 16, 10, 30).InUtc().ToInstant();
        Meeting expected = new("Name", expectedStart, expectedEnd);

        Meeting? actual = await client.CreateMeeting(BattleOfWits.Id, expected);
        
        Assert.Equal(expected, actual);
    }

    public void Dispose()
    {
        Factory?.Dispose();
    }
}
