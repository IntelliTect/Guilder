using NodaTime;

namespace Guilder.Shared;

public record class Meeting(string Name, string Description, Instant StartTimeInclusive, Instant EndTimeExclusive);