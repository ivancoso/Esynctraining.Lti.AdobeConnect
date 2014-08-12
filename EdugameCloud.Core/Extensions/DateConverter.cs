namespace EdugameCloud.Core.Extensions
{
    using System;

    public static class DateConverter
    {
        public static int ConvertToUTCTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            TimeSpan diff = date - origin.ToLocalTime();
            return (int)Math.Floor(diff.TotalSeconds);
        }

        public static int ConvertToTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}
