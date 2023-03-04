using Guilder.Client.Timeline;

namespace Guilder.Client.Services;

public class TimelineStore
{
    public LocalDateTime LowerBound { get; private set; }
    public LocalDateTime UpperBound { get; private set; }

    public List<TimelineSlot> SlotsToDisplay { get; private set; } = new List<TimelineSlot>();

    public IEnumerable<Meeting> Meetings { get; set; } = new List<Meeting>();

    public IClock Clock { get; }
    public ICurrentTimeZone CurrentTimeZone { get; }

    public static int SlotSizeInMinutes => 15;

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
            var slotEnd = slotStart.PlusMinutes(SlotSizeInMinutes);
            var meeting = InMeeting(slotStart, slotEnd);

            if (meeting is not null)
            {
                Console.WriteLine($"Meeting {meeting.Name} placed in {slotStart} - {slotEnd} slot");
            }

            SlotsToDisplay.Add(new TimelineSlot(slotStart, slotEnd, meeting));

            slotStart = slotEnd;
        }
    }

    public void SetMeetings(IEnumerable<Meeting> value)
    {
        Meetings = value;
        ReInitialize();
    }

    private Meeting? InMeeting(LocalDateTime slotStartInclusive, LocalDateTime slotEndExclusive)
    {
        var slotStartInclusiveUtc = slotStartInclusive.InZoneLeniently(CurrentTimeZone.TimeZone).ToInstant();
        var slotEndExclusiveUtc = slotEndExclusive.InZoneLeniently(CurrentTimeZone.TimeZone).ToInstant();

        // Doing an overlapping check
        return Meetings.FirstOrDefault(m => m.StartTimeInclusive < slotEndExclusiveUtc && m.EndTimeExclusive > slotStartInclusiveUtc);
    }
}
