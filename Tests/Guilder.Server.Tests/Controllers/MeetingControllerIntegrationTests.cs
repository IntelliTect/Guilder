using System;
using Microsoft.Extensions.DependencyInjection;

namespace Guilder.Server.Tests.Controllers;

public class MeetingControllerIntegrationTests : IDisposable
{
    (string Name, string Id) BattleOfWits = ("1. Battle of Wits", "3a02a800-1e8a-49ef-82f6-be60e1147fdd");

    private WebApplicationFactory Factory { get; } = new() { Connector = "MicrosoftGraph" };

    // [Fact]
    // Disabling because the created meetings are not deleted
    // and cannot be, even manually due to permissions.
    public async Task CreateMeeting_NewMeeting_Success()
    {
        // Used to identify a meeting for deletion.
        const string methodGuid = "{F209101B-77A3-4D64-B162-A2767EAD5639}";
        
        Factory.Connector = "MicrosoftGraph";
        
        MeetingClient client = new(Factory.CreateClient());

        DateTime now = DateTime.UtcNow;
        Instant expectedStart =
            new LocalDateTime(now.Year, now.Month, now.Day, 12, 0).InUtc().ToInstant();
        Instant expectedEnd =
            new LocalDateTime(now.Year, now.Month, now.Day, 12, 30).InUtc().ToInstant();
        Meeting expected = new($"Guilder - {methodGuid}", expectedStart, expectedEnd);

        Meeting? actual = await client.CreateMeeting(BattleOfWits.Id, expected);

        Assert.NotNull(actual);
        try
        {
            Assert.Equal(
                (expected.Name, expected.StartTimeInclusive, expected.EndTimeExclusive),
                (actual.Name, actual.StartTimeInclusive, actual.EndTimeExclusive));
        }
        finally
        {
            // TODO: This doesn't currently work.
            await client.DeleteMeetingAsync(BattleOfWits.Id, actual);
        }
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
