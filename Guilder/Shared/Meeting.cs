using NodaTime;

namespace Guilder.Shared;

public class Meeting
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Instant StartTimeInclusive { get; set; } = Instant.MinValue;
    public Instant EndTimeExclusive { get; set; } = Instant.MaxValue;
}
