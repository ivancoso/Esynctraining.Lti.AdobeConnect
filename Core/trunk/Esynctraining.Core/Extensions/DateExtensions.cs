namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// The date extensions.
    /// </summary>
    public static class DateExtensions
    {
        private static readonly DateTime _origin;
        private static readonly DateTime _originLocalTime;
        private static readonly DateTime _utcOrigin;

        static DateExtensions()
        {
            _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            _originLocalTime = _origin.ToLocalTime();
            _utcOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).ToLocalTime();
        }


        /// <summary>
        /// The age from birthday.
        /// </summary>
        /// <param name="birthDay">
        /// The birth day.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int32}"/>.
        /// </returns>
        public static int? AgeFromBirthday(this DateTime birthDay)
        {
            if (birthDay.Year == SqlDateTime.MinValue.Value.Year)
            {
                return null;
            }

            DateTime now = DateTime.Today;
            int age = now.Year - birthDay.Year;
            if (birthDay > now.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// The fix date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime AdaptToSQL(this DateTime dt)
        {
            if (SqlDateTime.MinValue.Value > dt)
            {
                return SqlDateTime.MinValue.Value;
            }

            if (SqlDateTime.MaxValue.Value < dt)
            {
                return SqlDateTime.MaxValue.Value;
            }

            return dt;
        }

        /// <summary>
        /// The fix date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime?"/>.
        /// </returns>
        public static DateTime? AdaptToSQL(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                if (SqlDateTime.MinValue.Value > dt.Value)
                {
                    return SqlDateTime.MinValue.Value;
                }

                if (SqlDateTime.MaxValue.Value < dt.Value)
                {
                    return SqlDateTime.MaxValue.Value;
                }
            }

            return dt;
        }
        
        /// <summary>
        /// The from micro seconds.
        /// </summary>
        /// <param name="microSec">
        /// The micro sec.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime FromMicroSeconds(this long microSec)
        {
            var milliSec = microSec / 1000;
            var startTime = new DateTime(1970, 1, 1);
            var time = TimeSpan.FromMilliseconds(milliSec);
            return startTime.Add(time);
        }

        /// <summary>
        /// The from micro seconds.
        /// </summary>
        /// <param name="dt">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static long ToMicroSeconds(this DateTime dt)
        {
            return (long)Math.Round((dt.ToUniversalTime() - new DateTime(1970, 1, 1).ToUniversalTime()).TotalMilliseconds) * 1000;
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
