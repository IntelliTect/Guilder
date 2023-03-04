using Guilder.Shared;

namespace Guilder.Server.Connectors;

public interface IMeetingRoomConnector
{
    Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId);
    Task<IReadOnlyList<Room>> GetRoomsAsync();
}
