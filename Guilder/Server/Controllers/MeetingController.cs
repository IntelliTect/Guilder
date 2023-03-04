using Guilder.Server.Connectors;
using Guilder.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;

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
    public async Task<IReadOnlyList<Meeting>> Get(string roomId)
    {
        await RoomConnector.GetRoomsAsync();
        return await RoomConnector.GetMeetingsAsync(roomId);
    }

    [HttpPost]
    public Task<Meeting> CreateMeeting(string roomId, Meeting meeting) => RoomConnector.CreateMeetingAsync(meeting);
}
