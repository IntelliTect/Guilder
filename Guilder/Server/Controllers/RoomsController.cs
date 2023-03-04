﻿using Guilder.Server.Connectors;
using Guilder.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Guilder.Server.Controllers;

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