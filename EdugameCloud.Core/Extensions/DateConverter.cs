namespace EdugameCloud.Core.Extensions
{
    using System;
    using System.Data.SqlTypes;

    using Esynctraining.Core.Extensions;

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
        /// The convert from unix time stamp.
        /// </summary>
        /// <param name="dt">
        /// The date time double.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ConvertFromUnixTimeStamp(this double dt)
        {
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(dt)).AdaptToSQL();
        }

        /// <summary>
        /// Convert a <c>DateTime</c> to a UNIX timestamp in milliseconds.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ConvertToUnixTimestamp(this DateTime value)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            if (value.Kind != DateTimeKind.Utc)
            {
                origin = origin.ToLocalTime();
            }

            return (value - origin).TotalMilliseconds;
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
