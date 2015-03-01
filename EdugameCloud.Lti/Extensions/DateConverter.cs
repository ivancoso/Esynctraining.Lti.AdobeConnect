using System;
namespace EdugameCloud.Lti.Extensions
{
    public static class DateConverter
    {
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
    }
}
