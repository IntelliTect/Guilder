using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.Calendar.GetSchedule;

using Room = Guilder.Shared.Room;

namespace Guilder.Server.Connectors.Graph;

public class GraphConnector : IMeetingRoomConnector
{
    private GraphServiceClient GraphClient { get; }

    public GraphConnector(GraphServiceClient graphClient)
    {
        GraphClient = graphClient;
    }

    public async Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
    {
        IReadOnlyList<Room> temp = await GetRoomsAsync();
        try
        {
            string userId = await GetUserId(temp.First(x => x.Id == "3a02a800-1e8a-49ef-82f6-be60e1147fdd"));
            //var result = await GraphClient.Users[userId.Value.First().Id].GetAsync((requestConfiguration) =>
            //{
            //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            //    requestConfiguration.QueryParameters.Expand = new[] { "calendar" };
            //});
            var result = await GraphClient.Users[userId].Calendar.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            });
            var calResult = await GraphClient.Users[userId].Calendar.Events.GetAsync();
        }
        catch (Exception)
        {
            throw;
        }
        return null!;
    }

    private async Task<string> GetUserId(Room room)
    {
        return ((await GraphClient.Users.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.QueryParameters.Filter = $"mail eq '{room.Email}'";
        }))?.Value?.First().Id) ?? throw new InvalidOperationException("User associated with room not found");
    }

    public async Task<IReadOnlyList<Meeting>> GetFreeBusyAsync(string roomId, Instant start, Instant end)
    {
        Room? room = await GetRoomAsync(roomId);

        if (room is null) return Array.Empty<Meeting>();
        const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        GetScheduleResponse? result = await GraphClient.Users[await GetUserId(room)].Calendar.GetSchedule.PostAsync(new GetSchedulePostRequestBody()
        {
            Schedules = new() { room.Email },
            StartTime = new DateTimeTimeZone
            {
                DateTime = start.ToDateTimeUtc().ToString(dateTimeFormat),
                TimeZone = "UTC"
            },
            EndTime = new DateTimeTimeZone
            {
                DateTime = end.ToDateTimeUtc().ToString(dateTimeFormat),
                TimeZone = "UTC"
            },
            AvailabilityViewInterval = 15,
        });
        if (result?.Value is { } freeBusyResults)
        {
            return freeBusyResults.SelectMany(scheduleInformation => scheduleInformation.ScheduleItems ?? Enumerable.Empty<ScheduleItem>())
                .Select(scheduleItem => new Meeting(scheduleItem.Subject ?? "Meeting",
                                                    Instant.FromDateTimeOffset(scheduleItem.Start.ToDateTimeOffset()),
                                                    Instant.FromDateTimeOffset(scheduleItem.End.ToDateTimeOffset()),
                                                    scheduleItem.Status.ToString())).ToList();
        }

        return Array.Empty<Meeting>();
    }

    public async Task<IReadOnlyList<Room>> GetRoomsAsync()
    {
        RoomCollectionResponse? response = await GraphClient.Places.GraphRoom
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Top = 10;
                });
        if (response?.Value is { } roomsResponse)
        {
            return roomsResponse.Where(room => room.Id is not null && room.EmailAddress is not null).Select(room => new Room(room.Id!, room.Nickname ?? room.DisplayName ?? $"Default Room {room.Id}", room.EmailAddress!)).ToList();
        }
        return Array.Empty<Room>();
    }
    
    
    public async Task<Room?> GetRoomAsync(string roomId)
    {
        IReadOnlyList<Room> rooms = await GetRoomsAsync();
        return rooms.FirstOrDefault(room => room.Id == roomId);
    }

    public async Task<Meeting> CreateMeetingAsync(string roomId, Meeting meeting)
    {
        Event appointment = new Event();
        appointment.Subject = meeting.Name;
        appointment.Start = meeting.StartTimeInclusive.ToDateTimeOffset().ToDateTimeTimeZone();
        appointment.End = meeting.EndTimeExclusive.ToDateTimeOffset().ToDateTimeTimeZone();
        appointment.Body = new ItemBody() { Content = meeting.Description };

        IReadOnlyList<Room> room = await GetRoomsAsync();
        string? userId = await GetUserId(room.First(item => item.Id == roomId));

        Event? result = await GraphClient.Users[userId].Calendar.Events.PostAsync(appointment);

        if (result is null)
        {
            throw new InvalidOperationException("Meeting creation failed");
        }

        return new Meeting(result.Subject??throw new InvalidOperationException("Subject is null"),
            Instant.FromDateTimeUtc(result.Start.ToDateTime().ToUniversalTime()),
            Instant.FromDateTimeUtc(result.End.ToDateTime().ToUniversalTime()),
            result.Body?.Content
            );
    }

    public async Task DeleteMeetingAsync(string roomId, Meeting meeting)
    {
        try
        {
            IReadOnlyList<Room> room = await GetRoomsAsync();
            string userId = (await GetUserId(room.First(item => item.Id == roomId)))
                ?? throw new InvalidOperationException("User associated with room not found");

            // TODO: This call is currently failing.
            List<Event> events = (await GraphClient.Users[userId].Calendar.Events.GetAsync())?.Value??
                throw new InvalidOperationException("No events found");

            foreach (Event @event in events)
            {
                Meeting eachMeeting = new(@event.Subject!,
                    Instant.FromDateTimeUtc(@event.Start.ToDateTime().ToUniversalTime()),
                    Instant.FromDateTimeUtc(@event.End.ToDateTime().ToUniversalTime()),
                    @event.Body?.Content);
                if (eachMeeting == meeting)
                {
                    await GraphClient.Users[userId].Calendar.Events[@event.Id].DeleteAsync();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

}


