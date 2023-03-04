namespace Guilder.Shared;

public record class Meeting(
    string Name, Instant StartTimeInclusive, Instant EndTimeExclusive, string? Description = null);
