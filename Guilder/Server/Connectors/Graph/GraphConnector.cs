using System.Reflection.Metadata.Ecma335;
using Azure.Identity;
using Guilder.Server.Authentication;
using Guilder.Shared;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using NodaTime;

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
            string? userId = ((await GraphClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"mail eq '{temp.First().Email}'";
            }))?.Value?.First().Id) ?? throw new InvalidOperationException("");
            //var result = await GraphClient.Users[userId.Value.First().Id].GetAsync((requestConfiguration) =>
            //{
            //    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            //    requestConfiguration.QueryParameters.Expand = new[] { "calendar" };
            //});
            var result = await GraphClient.Users[userId].Calendar.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            });
            //var calResult = await GraphClient.Users[result.Value.First().Id].Calendar.Events.GetAsync();
        }
        catch (Exception ex)
        {
        }
        return null!;
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

    public async Task<Meeting> CreateMeetingAsync(Meeting meeting)
    {
        // Room room = (await GetRoomsAsync()).First(item=>item.Id == roomId);

        //graphClient.Places.GraphRoom.
        //graphClient.Users["{user-id}"].Calendar.Events.PostAsync(new Event());

        return meeting;



    }
}


