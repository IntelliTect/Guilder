using Guilder.Shared;
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
        catch (Exception ex)
        {
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

    public async Task<IReadOnlyList<Meeting>> GetFreeBusyAsync(string roomId, DateTimeOffset start, DateTimeOffset end)
    {
        Room? room = await GetRoom(roomId);

        if (room is null) return Array.Empty<Meeting>();
        const string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        GetScheduleResponse? result = await GraphClient.Users[await GetUserId(room)].Calendar.GetSchedule.PostAsync(new GetSchedulePostRequestBody()
        {
            Schedules = new() { room.Email },
            StartTime = new DateTimeTimeZone
            {
                DateTime = start.ToUniversalTime().ToString(dateTimeFormat),
                TimeZone = "UTC"
            },
            EndTime = new DateTimeTimeZone
            {
                DateTime = end.ToUniversalTime().ToString(dateTimeFormat),
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

    private async Task<Room?> GetRoom(string id)
    {
        IReadOnlyList<Room> rooms = await GetRoomsAsync();
        return rooms.FirstOrDefault(room => room.Id == id);
    }

    public async Task<Meeting> CreateMeetingAsync(string roomId, Meeting meeting)
    {
        try
        {
            // Room room = (await GetRoomsAsync()).First(item=>item.Id == roomId);

            //graphClient.Places.GraphRoom.
            //graphClient.Users["{user-id}"].Calendar.Events.PostAsync(new Event());

            OnlineMeeting requestBody = new OnlineMeeting
            {
                StartDateTime = meeting.StartTimeInclusive.ToDateTimeOffset(),
                EndDateTime = meeting.EndTimeExclusive.ToDateTimeOffset(),
                Subject = meeting.Name
            };

            Event appointment = new Event();
            appointment.Subject = meeting.Name;
            // appointment.Body = new ItemBody(meeting.Description, ContentType.Text);
            appointment.Start = meeting.StartTimeInclusive.ToDateTimeOffset().ToDateTimeTimeZone();
            appointment.End = meeting.EndTimeExclusive.ToDateTimeOffset().ToDateTimeTimeZone();

            IReadOnlyList<Room> room = await GetRoomsAsync();
            string? userId = await GetUserId(room.First(item => item.Id == roomId));

            //var result = await GraphClient.Users[userId.Value.First().Id].GetAsync((requestConfiguration) => 
            //{ 
            //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual"); 
            //    requestConfiguration.QueryParameters.Expand = new[] { "calendar" }; 
            //}); 
            Calendar? calendar = await GraphClient.Users[userId].Calendar.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            });
            //var calResult = await GraphClient.Users[result.Value.First().Id].Calendar.Events.GetAsync(); 

            //Task<OnlineMeetingCollectionResponse?> user =
            //    GraphClient.Users[userId].Calendar.GetAsync( (requestConfiguration) =>
            //{
            //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            //});
            //int beforeCount = calendar.Events.Count;
            //calendar.Events.Add(appointment);
            //int afterCount = calendar.Events.Count;


            //var result = await me.OnlineMeetings.PostAsync(requestBody);
            return meeting;


        }
        catch (Exception ex)
        {
            throw;
        }
    }
}


