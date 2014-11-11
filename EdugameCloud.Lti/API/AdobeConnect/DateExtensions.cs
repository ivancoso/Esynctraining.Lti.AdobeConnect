namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;

    /// <summary>
    /// The date extensions.
    /// </summary>
    public static class DateExtensions
    {
        #region Static Fields

        /// <summary>
        /// The date time 1951.
        /// </summary>
        private static readonly DateTime dt1951 = new DateTime(1951, 1, 1);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The fix ac value.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime?"/>.
        /// </returns>
        public static DateTime? FixACValue(this DateTime dt)
        {
            return dt < dt1951 ? (DateTime?)null : dt;
        }

        #endregion
    }
}