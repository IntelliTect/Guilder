using NodaTime;

namespace Guilder.Client.Timeline
{
    record class TimelineSlot(LocalDateTime startInclusive, LocalDateTime endExclusive);
}
