namespace Guilder.Shared;

public record class Meeting(
    string Name, Instant StartTimeInclusive, Instant EndTimeExclusive, string? Description = null)
{
    public virtual bool Equals(Meeting? other) =>
        other is not null && (ReferenceEquals(this, other) ||
    (Name, StartTimeInclusive, EndTimeExclusive) == (
        other.Name, other.StartTimeInclusive, other.EndTimeExclusive));
    public override int GetHashCode() => HashCode.Combine(Name, StartTimeInclusive, EndTimeExclusive);
    
    public bool OverlapsWith(Meeting other) =>
            StartTimeInclusive < other.EndTimeExclusive && EndTimeExclusive > other.StartTimeInclusive;
}
