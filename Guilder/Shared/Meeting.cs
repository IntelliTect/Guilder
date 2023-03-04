using NodaTime;

namespace Guilder.Shared;

public record class Meeting(string Name, Instant StartTimeInclusive, Instant EndTimeExclusive, string? Description) {

    public Meeting(string roomId, Instant expectedStart, Instant expectedEnd) : this(
        roomId, expectedStart, expectedEnd, null) { }
    

}
