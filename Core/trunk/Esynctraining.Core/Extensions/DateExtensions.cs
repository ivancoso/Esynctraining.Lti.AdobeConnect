namespace Esynctraining.Core.Extensions
{
    using System;
    using System.Data.SqlTypes;

    /// <summary>
    /// The date extensions.
    /// </summary>
    public static class DateExtensions
    {
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
        /// The to EST.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ToEst(this DateTime dateTime)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), easternZone);
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

    }
}
