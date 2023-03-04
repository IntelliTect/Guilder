using Guilder.Shared;
using NodaTime;
using System.Collections.Generic;

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
            var currentSlotStart =  GetSlotStart(_clock.GetCurrentInstant());

            return Task.FromResult((IReadOnlyList<Meeting>)new List<Meeting>
            {
                new()
                {
                    Name = "Meeting 1",
                    Description = "Talking about the app",
                    StartTimeInclusive = currentSlotStart,
                    EndTimeExclusive = currentSlotStart.Plus(Duration.FromMinutes(30)),
                }
            });
        }

        // Not supporting any timezones that are not offest by a multiple of 30 minutes
        private Instant GetSlotStart(Instant instant)
        {
            var dateTime = instant.InUtc().LocalDateTime;

            return new LocalDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / 30) * 30).InUtc().ToInstant();
        }
    }
}
