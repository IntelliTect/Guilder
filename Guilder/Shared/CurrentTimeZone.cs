namespace Guilder.Shared;

public record CurrentTimeZone(DateTimeZone TimeZone) : ICurrentTimeZone;
