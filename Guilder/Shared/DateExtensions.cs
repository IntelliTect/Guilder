using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guilder.Shared
{
    public static class DateExtensions
    {
        // Slot starts dont support any time zones that are not offset by a multiple of 30 minutes

        public static Instant GetSlotStart(this Instant instant)
        {
            return instant.InUtc().LocalDateTime.GetSlotStart().InUtc().ToInstant();
        }

        public static LocalDateTime GetSlotStart(this LocalDateTime dateTime)
        {
            return new LocalDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / 30) * 30);
        }
    }
}
