using System.ComponentModel;
using Guilder.Server.Connectors;
using Microsoft.AspNetCore.Mvc;

namespace Guilder.Server.Controllers;

[ApiController]
[Route("room/{roomId}/[controller]")]
public class MeetingsController
{
    public IMeetingRoomConnector RoomConnector { get; }

    public MeetingsController(IMeetingRoomConnector roomConnector)
    {
        RoomConnector = roomConnector;
    }

    [HttpGet("FreeBusy")]
    // Default: Battle Of Wits Room Id
    public async Task<IReadOnlyList<Meeting>> GetFreeBusy(
        [DefaultValue("3a02a800-1e8a-49ef-82f6-be60e1147fdd")] string roomId,
        Instant start, Instant end)
    {
        return await RoomConnector.GetFreeBusyAsync(roomId, start, end);
    }

    [HttpGet()]
    // Default: Battle Of Wits Room Id
    public async Task<IReadOnlyList<Meeting>> GetMeetings(
        [DefaultValue("3a02a800-1e8a-49ef-82f6-be60e1147fdd")] string roomId)
    {
        return await RoomConnector.GetMeetingsAsync(roomId);
    }

    [HttpPost]
    public async Task<Meeting> CreateMeetingAsync(string roomId, Meeting meeting) =>
        await RoomConnector.CreateMeetingAsync(roomId, meeting);

    [HttpPost("delete")]
    // [HttpDelete] - Currently Meeting doesn't have an id so using Post.
    public async Task DeleteMeetingAsync(string roomId, Meeting meeting) =>
        await RoomConnector.DeleteMeetingAsync(roomId, meeting);
}
