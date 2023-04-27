using System;
using System.Collections.Concurrent;

namespace Services.Utiltities
{
    public static class CommonUtils
    {
        private static ConcurrentDictionary<Guid, int> EntityCountStates = new ConcurrentDictionary<Guid, int>();

        public static DateTime? EnsureStartOfDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            return StartOfDate(dateTime.Value.ToUniversalTime());
        }

        public static DateTime StartOfDate(this DateTime dateTime)
        {
            return dateTime.AddHours(-dateTime.Hour)
                             .AddMinutes(-dateTime.Minute)
                             .AddSeconds(-dateTime.Second);
        }

        public static DateTime? EnsureEndOfDate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            return EndOfDate(dateTime.Value.ToUniversalTime());
        }

        public static DateTime EndOfDate(this DateTime dateTime)
        {
            return dateTime.AddHours(23 - dateTime.Hour)
                           .AddMinutes(59 - dateTime.Minute)
                           .AddSeconds(59 - dateTime.Second);
        }

        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            int diff = (7 +(dt.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
