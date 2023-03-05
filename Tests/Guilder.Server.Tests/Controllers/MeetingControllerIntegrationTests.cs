namespace Guilder.Server.Tests.Controllers;

public class MeetingControllerIntegrationTests : IDisposable
{
    (string Name, string Id) BattleOfWits = ("1. Battle of Wits", "3a02a800-1e8a-49ef-82f6-be60e1147fdd");

    private WebApplicationFactory Factory { get; } = new() { Connector = "MicrosoftGraph" };

    [Fact]
    public async Task CreateMeeting_NewMeeting_Success()
    {
        Factory.Connector = "MicrosoftGraph";
        
        MeetingClient client = new(Factory.CreateClient());

        Instant expectedStart = new LocalDateTime(2022, 3, 16, 10, 0).InUtc().ToInstant();
        Instant expectedEnd = new LocalDateTime(2022, 3, 16, 10, 30).InUtc().ToInstant();
        Meeting expected = new("Name", expectedStart, expectedEnd);

        Meeting? actual = await client.CreateMeeting(BattleOfWits.Id, expected);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetFreeBusy_WithRoom_GetsAvailability()
    {
        MeetingClient client = new(Factory.CreateClient());

        var start = Instant.FromDateTimeUtc(DateTime.UtcNow.Date);
        var end = Instant.FromDateTimeUtc(DateTime.UtcNow.Date.AddDays(1));
        
        IReadOnlyList<Meeting> meetings = await client.GetFreeBusyForRoomId(BattleOfWits.Id, start, end);

        Assert.True(meetings.Any());
    }

    public void Dispose() => Factory?.Dispose();
}
