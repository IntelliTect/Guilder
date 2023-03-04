using Guilder.Shared;
using NodaTime;

namespace Guilder.Server.Connectors.Fake
{
    public class FakeCurrentMeetingConnector : IMeetingRoomConnector
    {
        private readonly IClock _clock;

        public FakeCurrentMeetingConnector(IClock clock)
        {
            _clock = clock;
        }

        public Task<Meeting> CreateMeetingAsync(Meeting meeting) => throw new NotImplementedException();
        public Task<IReadOnlyList<Meeting>> GetFreeBusyAsync(string roomId, DateTimeOffset start, DateTimeOffset end) => throw new NotImplementedException();

        public Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
        {
            var currentSlotStart = _clock.GetCurrentInstant().GetSlotStart();

            return Task.FromResult((IReadOnlyList<Meeting>)new List<Meeting>()

            {
                new ("Meeting 1",
                    currentSlotStart,
                    currentSlotStart.Plus(Duration.FromMinutes(30)),
                    "Talking about the app")

            });

        }

        public Task<IReadOnlyList<Room>> GetRoomsAsync()
        {
            return Task.FromResult((IReadOnlyList<Room>)new List<Room>()
            {
                new ("1", "Conference Room", "hi@intellitect.com")

            });
        }
    }
}
