﻿using System.Text.Json;
using NodaTime.Serialization.SystemTextJson;
using static NodaTime.TimeZones.ZoneEqualityComparer;
using static System.Net.WebRequestMethods;

namespace Guilder.Shared;

public class MeetingClient
{
    private static JsonSerializerOptions Options { get; }
        = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    private HttpClient Http { get; }

    public MeetingClient(HttpClient http)
    {
        Http = http;
    }

    public async Task<IReadOnlyList<Meeting>> GetMeetingsForRoomId(string roomId)
    {
        var response = await Http.GetFromJsonAsync<IReadOnlyList<Meeting>>(
            $"room/{roomId}/meetings", Options);

        return response ?? Array.Empty<Meeting>();
    }

    public async Task<IReadOnlyList<Meeting>> GetFreeBusyForRoomId(string roomId, Instant start, Instant end)
    {
        var response = await Http.GetFromJsonAsync<IReadOnlyList<Meeting>>(
            $"room/{roomId}/meetings/FreeBusy?start={start}&end={end}", Options);

        return response ?? Array.Empty<Meeting>();
    }

    public async Task<Meeting?> CreateMeeting(string roomId, Meeting meeting)
    {
        HttpResponseMessage response =
            await Http.PostAsJsonAsync(
                $"room/{roomId}/meetings", meeting, Options);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Meeting>(Options);
        }
        return null;
    }

    public async Task<Room?> GetRoomAsync(string roomId) =>
        await Http.GetFromJsonAsync<Room?>($"rooms/{roomId}", Options);

    public async Task DeleteMeetingAsync(string roomId, Meeting meeting) =>
        await Http.PostAsJsonAsync($"room/{roomId}/meetings/delete", meeting, Options);
}

