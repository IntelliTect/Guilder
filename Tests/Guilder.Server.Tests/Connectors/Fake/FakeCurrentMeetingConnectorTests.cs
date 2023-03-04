using Guilder.Server.Connectors.Fake;

namespace Guilder.Server.Tests.Connectors.Fake;

public class FakeCurrentMeetingConnectorTests
{
    [Fact]
    public async Task GetMeetingsAsync_NowInMiddleOfSlot_StartEndOnSlotBounds()
    {
        var mocker = new AutoMocker();
        mocker.GetMock<IClock>().Setup(x => x.GetCurrentInstant()).Returns(new LocalDateTime(2022, 3, 16, 10, 8).InUtc().ToInstant());

        var connector = mocker.CreateInstance<FakeCurrentMeetingConnector>();

        var meetings = await connector.GetMeetingsAsync("1");

        var expectedStart = new LocalDateTime(2022, 3, 16, 10, 0).InUtc().ToInstant();
        var expectedEnd = new LocalDateTime(2022, 3, 16, 10, 30).InUtc().ToInstant();

        var firstMeeting = meetings.First();

        Assert.Equal(expectedStart, firstMeeting.StartTimeInclusive);
        Assert.Equal(expectedEnd, firstMeeting.EndTimeExclusive);
    }

    [Fact]
    public async Task GetMeetingsAsync_HasFutureMeetingWithGap()
    {
        var mocker = new AutoMocker();
        mocker.GetMock<IClock>().Setup(x => x.GetCurrentInstant()).Returns(new LocalDateTime(2022, 3, 16, 10, 8).InUtc().ToInstant());

        var connector = mocker.CreateInstance<FakeCurrentMeetingConnector>();

        var meetings = await connector.GetMeetingsAsync("1");

        var firstMeeting = meetings.First();
        var nextMeeting = meetings.Skip(1).First();

        Assert.InRange(nextMeeting.StartTimeInclusive,
            firstMeeting.StartTimeInclusive.Plus(Duration.FromMinutes(60)), firstMeeting.StartTimeInclusive.Plus(Duration.FromMinutes(300)));
    }
}
