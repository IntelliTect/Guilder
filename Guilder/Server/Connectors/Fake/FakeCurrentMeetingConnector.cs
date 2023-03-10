namespace Guilder.Server.Connectors.Fake;

public class FakeCurrentMeetingConnector : IMeetingRoomConnector
{
    private readonly IClock _clock;

    public FakeCurrentMeetingConnector(IClock clock)
    {
        _clock = clock;
    }

    public Task<Meeting> CreateMeetingAsync(string roomId, Meeting meeting) => Task.FromResult(meeting);
    public Task DeleteMeetingAsync(string roomId, Meeting meeting) => Task.CompletedTask;

    public Task<IReadOnlyList<Meeting>> GetFreeBusyAsync(string roomId, Instant start, Instant end)
    {
        return GetMeetingsAsync(roomId);
    }

    public Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
    {
        var currentSlotStart = _clock.GetCurrentInstant().GetSlotStart();

        return Task.FromResult((IReadOnlyList<Meeting>)new List<Meeting>()

        {
            new ("Meeting 1",
                currentSlotStart,
                currentSlotStart.Plus(Duration.FromMinutes(30)),
                "Talking about the app"),

                        new ("Meeting 2",
                currentSlotStart.Plus(Duration.FromMinutes(90)),
                currentSlotStart.Plus(Duration.FromMinutes(90 + 45)),
                "Talking about the app's utter failure"),

        });

    }

    public Task<Room?> GetRoomAsync(string roomId) =>
        GetRoomsAsync().ContinueWith(
            t => t.Result.First(r => r.Id == roomId) ??
            new("-1", "Unknown Fake Room", "hi@intellitect.com")) as Task<Room?>;

    public Task<IReadOnlyList<Room>> GetRoomsAsync()
    {
        return Task.FromResult((IReadOnlyList<Room>)new List<Room>()
        {
            new ("1", "Conference Room", "hi@intellitect.com"),
            new ("2", "Other Room", "hi@intellitect.com"),
            new ("3", "Yup Room", "hi@intellitect.com")

        });
    }
}
