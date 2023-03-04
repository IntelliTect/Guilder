using Guilder.Server.Connectors.Graph;
using Guilder.Shared;
using NodaTime;

namespace Guilder.Server.Tests.Connectors;

public class GraphConnectorTests
{
    [Fact]
    public async void CreateMeeting_NewMeeting_Success()
    {

        Instant expectedStart = new LocalDateTime(2022, 3, 16, 10, 0).InUtc().ToInstant();
        Instant expectedEnd = new LocalDateTime(2022, 3, 16, 10, 30).InUtc().ToInstant();
        Meeting expected = new("1", expectedStart, expectedEnd);

        AutoMocker mocker = new();
        mocker.GetMock<IClock>().Setup(x => x.GetCurrentInstant()).Returns(new LocalDateTime(2022, 3, 16, 10, 8).InUtc().ToInstant());

        GraphConnector connector = mocker.CreateInstance<GraphConnector>();

        Meeting actual = await connector.CreateMeetingAsync("1", expectedStart, expectedEnd);

        Assert.Equal(expected, actual);
    }
}