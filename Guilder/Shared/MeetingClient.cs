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
    public class MeetingClient
    {
        private readonly HttpClient _http;

        public MeetingClient(HttpClient Http)
        {
            _http = Http;
        }

        public async Task<IEnumerable<Meeting>> GetMeetingsForRoomId(string roomId)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var response = await _http.GetFromJsonAsync<IEnumerable<Meeting>>($"room/{roomId}/meeting", options);

            return response ?? new List<Meeting>();
        }
    }
}
