using Guilder.Shared;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace Guilder.Client.Services;

public class TimelineStore
{
    public LocalDateTime LowerBound { get; private set; }
    public LocalDateTime UpperBound { get; private set; }

    private IEnumerable<Meeting> AllMeetings { get; set; } = new List<Meeting>();

    public IClock Clock { get; }
    public ICurrentTimeZone CurrentTimeZone { get; }

    public TimelineStore(IClock clock, ICurrentTimeZone currentTimeZone)
    {
        Clock = clock;
        CurrentTimeZone = currentTimeZone;
    }

    public void ReInitialize()
    {
        var now = Clock.GetCurrentInstant();
        var currentSlotStartLocal = now.GetSlotStart().InZone(CurrentTimeZone.TimeZone).LocalDateTime;

        LowerBound = currentSlotStartLocal.PlusMinutes(-30);
        UpperBound = currentSlotStartLocal.PlusMinutes(4 * 60);
    }

    public void SetMeetings(IEnumerable<Meeting> value)
    {
        AllMeetings = value;
        ReInitialize();
    }
}
