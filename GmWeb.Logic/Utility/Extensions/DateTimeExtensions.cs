using System;
using System.Globalization;
using GmWeb.Logic.Enums;

namespace GmWeb.Logic.Utility.Extensions.Chronometry
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime _EpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime EpochDateTime => _EpochDateTime;
        public static DateTime FromUnixTimeSeconds(this float timestamp)
            => EpochDateTime.AddSeconds(timestamp);
        public static DateTime FromUnixTimeSeconds(this double timestamp)
            => EpochDateTime.AddSeconds(timestamp);
        public static DateTime FromUnixTimeSeconds(this int timestamp)
            => EpochDateTime.AddSeconds(timestamp);
        public static DateTime FromUnixTimeSeconds(this long timestamp)
            => EpochDateTime.AddSeconds(timestamp);
        public static long ToUnixTimeSeconds(this DateTime date)
            => (long)(date.ToUniversalTime() - EpochDateTime).TotalSeconds;

        public static (DateTime, DateTime) ToWindow(this DateTime center, TimeSpan windowSize)
            => (center.Add(-windowSize), center.Add(windowSize));

        public static DateTime AddWeeks(this DateTime dt, int weeks) => dt.AddDays(weeks * 7);
        public static DateTime AddQuarters(this DateTime dt, int quarters) => dt.AddMonths(quarters * 3);
    }
}
