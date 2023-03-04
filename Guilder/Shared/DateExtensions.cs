namespace Guilder.Shared
{
    public static class DateExtensions
    {
        // Slot starts dont support any time zones that are not offset by a multiple of 30 minutes

        public static Instant GetSlotStart(this Instant instant) => instant.InUtc().LocalDateTime.GetSlotStart().InUtc().ToInstant();

        public static LocalDateTime GetSlotStart(this LocalDateTime dateTime) => new LocalDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / 30) * 30);
    }
}
