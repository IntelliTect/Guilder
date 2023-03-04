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
        //EventCollectionResponse? response = await graphClient.Users.GetByIds.PostAsync.GetAsync((requestConfiguration) =>
        //{
        //    requestConfiguration.QueryParameters.Select = new string[] { "subject", "organizer", "attendees", "start", "end", "location" };
        //    requestConfiguration.Headers.Add("Prefer", "outlook.timezone=\"Pacific Standard Time\"");
        //});

        throw new NotImplementedException();
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
            return roomsResponse.Where(room => room.Id is not null).Select(room => new Room(room.Id!, room.Nickname ?? room.DisplayName ?? $"Default Room {room.Id}")).ToList();
        }
        return Array.Empty<Room>();
    }

    public async Task<Meeting> CreateMeetingAsync(Meeting meeting)
    {
        // Room room = (await GetRoomsAsync()).First(item=>item.Id == roomId);

        //graphClient.Places.GraphRoom.
        //graphClient.Users["{user-id}"].Calendar.Events.PostAsync(new Event());

        return meeting;



    }
}


