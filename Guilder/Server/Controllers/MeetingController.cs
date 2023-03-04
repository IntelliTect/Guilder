using System.ComponentModel;
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

    [HttpGet("/{date}")]
    public async Task<IReadOnlyList<Meeting>> Get([DefaultValue("f9f421b0-11dd-438e-89d0-1cfe1aac2467")] string roomId, DateOnly date)
    {
        return await RoomConnector.GetFreeBusyAsync(roomId, date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue));
    }

    [HttpPost]
    public Task<Meeting> CreateMeeting(string roomId, Meeting meeting) => RoomConnector.CreateMeetingAsync(meeting);
}
