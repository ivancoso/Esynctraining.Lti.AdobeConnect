namespace PDFAnnotation.Core.Business
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The time extensions.
    /// </summary>
    public static class TimeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to hours with round up.
        /// </summary>
        /// <param name="totalMinutes">
        /// The total minutes.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float ToHoursWithRoundUp(this float totalMinutes)
        {
            int totalHours = (int)totalMinutes / 60;
            var minutesLeft = totalMinutes - (totalHours * 60);
            if (totalHours == 0)
            {
                if (minutesLeft < 2)
                {
                    return 0;
                }
                return 1;
            }
            return totalHours + GetOffset(minutesLeft);
        }

        /// <summary>
        /// The date time to unix timestamp.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ToUnixEpoch(this DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
        }

        /// <summary>
        /// The unix time stamp to date time.
        /// </summary>
        /// <param name="unixTimeStamp">
        /// The unix time stamp.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public static DateTime ToDateTime(this double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;
        }

        /// <summary>
        /// The unix time stamp to date time local.
        /// </summary>
        /// <param name="unixTimeStamp">
        /// The unix time stamp.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ToDateTimeLocal(this double unixTimeStamp)
        {
            return unixTimeStamp.ToDateTime().ToLocalTime();
        }

        /// <summary>
        /// The to hours with round up.
        /// </summary>
        /// <param name="totalMinutes">
        /// The total minutes.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float ToHoursWithRoundUp(this double totalMinutes)
        {
            return ((float)totalMinutes).ToHoursWithRoundUp();
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// The get offset.
        /// </summary>
        /// <param name="minutesLeft">
        /// The minutes left.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        private static float GetOffset(float minutesLeft)
        {
            if (minutesLeft <= 0)
            {
                return 0;
            }
            return minutesLeft <= 30 ? 0.5f : 1;
        }

        #endregion
    }
}