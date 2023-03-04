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

            throw new NotImplementedException();

        }
    }
}
