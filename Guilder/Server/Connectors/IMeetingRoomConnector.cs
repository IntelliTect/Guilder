using Guilder.Shared;

namespace Guilder.Server.Connectors;

public interface IMeetingRoomConnector
{
    Task<Meeting> CreateMeetingAsync(Meeting meeting);
    Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId);
    Task<IReadOnlyList<Room>> GetRoomsAsync();
}
