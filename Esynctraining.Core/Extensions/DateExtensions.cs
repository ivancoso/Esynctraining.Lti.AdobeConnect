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
    }
}
