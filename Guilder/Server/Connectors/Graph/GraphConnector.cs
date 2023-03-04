using Guilder.Shared;

namespace Guilder.Server.Connectors.Graph;

public class GraphConnector : IMeetingRoomConnector
{
    public Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string roomId)
    {
        throw new NotImplementedException();
    }
}
