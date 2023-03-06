namespace Guilder.Server.Connectors;

public interface IMeetingRoomConnector
{
    Task<Meeting> CreateMeetingAsync(string roomId, Meeting meeting);

    Task DeleteMeetingAsync(string roomId, Meeting meeting);
        
    Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId);
    Task<IReadOnlyList<Room>> GetRoomsAsync();
    Task<Room?> GetRoomAsync(string roomId);
    Task<IReadOnlyList<Meeting>> GetFreeBusyAsync(string roomId, Instant start, Instant end);
}
