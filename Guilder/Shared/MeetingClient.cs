using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Guilder.Shared
{
    public record class MeetingClient(HttpClient Http)
    {
        private HttpClient Http { get; } = Http;

        public async Task<IEnumerable<Meeting>> GetMeetingsForRoomId(string roomId)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var response = await _http.GetFromJsonAsync<IEnumerable<Meeting>>($"room/{roomId}/meeting", options);

            return response ?? new List<Meeting>();
        }
    }
}
