using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Guilder.Shared
{
    public class MeetingClient
    {
        private HttpClient Http { get; }

        public MeetingClient(HttpClient http)
        {
            Http = http;
        }

        public async Task<IEnumerable<Meeting>> GetMeetingsForRoomId(string roomId)
        {
            var options = new JsonSerializerOptions(
                JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var response = await Http.GetFromJsonAsync<IEnumerable<Meeting>>(
                $"room/{roomId}/meeting", options);

            return response ?? new List<Meeting>();
        }
    }
}
