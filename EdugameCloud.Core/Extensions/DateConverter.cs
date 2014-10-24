namespace EdugameCloud.Core.Extensions
{
    using System;

    /// <summary>
    /// The date converter.
    /// </summary>
    public static class DateConverter
    {
        /// <summary>
        /// The convert to utc timestamp.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ConvertToUTCTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            TimeSpan diff = date - origin.ToLocalTime();
            return (int)Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// The convert to timestamp.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ConvertToTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}
