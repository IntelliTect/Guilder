using Guilder.Server.Connectors;
using Guilder.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Guilder.Server.Controllers;

[ApiController]
[Route("room/{roomId}/[controller]")]
public class MeetingController
{
    public IMeetingRoomConnector RoomConnector { get; }

    public MeetingController(IMeetingRoomConnector roomConnector)
    {
        RoomConnector = roomConnector;
    }


    [HttpGet]
    public Task<IReadOnlyList<Meeting>> Get(string roomId)
    {
        return RoomConnector.GetMeetingsAsync(roomId);
    }
}
