using Guilder.Client.Timeline;
using Guilder.Shared;
using NodaTime;

namespace Guilder.Client.Services;

public class TimelineStore
{
    public LocalDateTime LowerBound { get; private set; }
    public LocalDateTime UpperBound { get; private set; }

    public List<TimelineSlot> SlotsToDisplay { get; private set; } = new List<TimelineSlot>();

    private IEnumerable<Meeting> Meetings { get; set; } = new List<Meeting>();

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

        var slotStart = LowerBound;

        SlotsToDisplay.Clear();

        while (slotStart < UpperBound)
        {
            SlotsToDisplay.Add(new TimelineSlot(slotStart, slotStart.PlusMinutes(30)));

            slotStart = slotStart.PlusMinutes(30);
        }
    }

    public void SetMeetings(IEnumerable<Meeting> value)
    {
        Meetings = value;
        ReInitialize();
    }
}
