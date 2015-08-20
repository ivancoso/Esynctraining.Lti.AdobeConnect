namespace EdugameCloud.Core.Extensions
{
    using System;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The date converter.
    /// </summary>
    public static class DateConverter
    {
        private static readonly DateTime _origin;
        private static readonly DateTime _originLocalTime;
        private static readonly DateTime _utcOrigin;


        static DateConverter()
        {
            _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            _originLocalTime = _origin.ToLocalTime();
            _utcOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).ToLocalTime();
        }
        

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
            //TimeSpan diff = date - _utcOrigin;
            return (int)Math.Floor((date - _utcOrigin).TotalSeconds);
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
            return _origin.Add(TimeSpan.FromMilliseconds(dt)).AdaptToSQL();
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
            if (value.Kind != DateTimeKind.Utc)
            {
                return (value - _originLocalTime).TotalSeconds * 1000;
            }
            return (value - _origin).TotalSeconds * 1000;
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
            //TimeSpan diff = date - _origin;
            return (int)Math.Floor((date - _origin).TotalSeconds);
        }

        public static DateTime ConvertToClientTime(this DateTime date, int timezoneOffset)
        {
            if (timezoneOffset != 0)
            {
                var offset = int.Parse(timezoneOffset.ToString());
                date = date.AddMinutes(-1 * offset);

                return date;
            }

            return date.ToLocalTime();
        }
    }

}
