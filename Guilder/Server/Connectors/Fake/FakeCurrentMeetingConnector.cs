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

        public Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
        {
            var currentSlotStart = GetSlotStart(_clock.GetCurrentInstant());

            return Task.FromResult((IReadOnlyList<Meeting>)new List<Meeting>()
            {
                new ("Meeting 1",
                    currentSlotStart,
                    currentSlotStart.Plus(Duration.FromMinutes(30)),
                    "Talking about the app")
            });
        }

        public Task<IReadOnlyList<Room>> GetRoomsAsync(string roomId)
        {
            throw new NotImplementedException();
        }

        // Not supporting any time zones that are not offset by a multiple of 30 minutes
        private Instant GetSlotStart(Instant instant)
        {
            var dateTime = instant.InUtc().LocalDateTime;

            return new LocalDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / 30) * 30).InUtc().ToInstant();
        }
    }
}
