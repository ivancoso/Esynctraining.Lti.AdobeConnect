using System;

namespace EdugameCloud.Lti.Core.Utils
{
    public static class DateTimeHelper
    {
        private static readonly DateTime dt1951 = new DateTime(1951, 1, 1);

        public static DateTime? ConvertToEST(DateTime time)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return ConvertTimeFromUtc(time, easternZone);
        }

        public static DateTime? ConvertTimeFromUtc(DateTime dt, TimeZoneInfo timeZone)
        {
            var tmp = dt < dt1951 
                ? (DateTime?)null 
                : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);

            if (tmp.HasValue)
            {
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(tmp.Value, timeZone), DateTimeKind.Utc);
            }
            return null;
        }
    }
}
