using NodaTime;

namespace Guilder.Shared;

public interface ICurrentTimeZone
{
    DateTimeZone TimeZone { get; } 
}
