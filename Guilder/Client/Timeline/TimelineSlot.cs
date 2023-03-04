using NodaTime;

namespace Guilder.Client.Timeline
{
    public record class TimelineSlot(LocalDateTime StartInclusive, LocalDateTime EndExclusive);
}
