using System.ComponentModel;
using Guilder.Server.Connectors;
using Guilder.Shared;
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

    [HttpGet("FreeBusy/{date}")]
    // Default: Humperdink Castle Room Id
    public async Task<IReadOnlyList<Meeting>> GetFreeBusy([DefaultValue("3a02a800-1e8a-49ef-82f6-be60e1147fdd")] string roomId, DateOnly date)
    {
        return await RoomConnector.GetFreeBusyAsync(roomId, date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue));
    }

    [HttpGet("{date}")]
    // Default: Humperdink Castle Room Id
    public async Task<IReadOnlyList<Meeting>> GetMeetings([DefaultValue("3a02a800-1e8a-49ef-82f6-be60e1147fdd")] string roomId, DateOnly date)
    {
        return await RoomConnector.GetMeetingsAsync(roomId);
    }

    [HttpPost]
    public Task<Meeting> CreateMeeting(string roomId, Meeting meeting) => RoomConnector.CreateMeetingAsync(meeting);
}

[ApiController]
[Route("rooms")]
public class RoomsController
{
    public IMeetingRoomConnector RoomConnector { get; }

    public RoomsController(IMeetingRoomConnector roomConnector)
    {
        RoomConnector = roomConnector;
    }

    [HttpGet]
    public async Task<IReadOnlyList<Room>> Get()
    {
        return await RoomConnector.GetRoomsAsync();
    }
}
