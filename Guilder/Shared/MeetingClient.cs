using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Guilder.Shared;

public class MeetingClient
{
    private HttpClient Http { get; }

    public MeetingClient(HttpClient http)
    {
        Http = http;
    }

    public async Task<IReadOnlyList<Meeting>> GetMeetingsForRoomId(string roomId)
    {
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        var response = await Http.GetFromJsonAsync<IReadOnlyList<Meeting>>(
            $"room/{roomId}/Meetings", options);

        return response ?? new List<Meeting>();
    }

    public async Task<IReadOnlyList<Meeting>> GetFreeBusyForRoomId(string roomId, LocalDate date)
    {
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        var response = await Http.GetFromJsonAsync<IReadOnlyList<Meeting>>(
            $"room/{roomId}/Meetings/FreeBusy/{date}", options);

        return response ?? Array.Empty<Meeting>();
    }
}
