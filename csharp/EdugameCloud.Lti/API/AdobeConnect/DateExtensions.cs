namespace EdugameCloud.Lti.API.AdobeConnect
{
    using System;

    /// <summary>
    /// The date extensions.
    /// </summary>
    internal static class DateExtensions
    {
        #region Static Fields

        /// <summary>
        /// The date time 1951.
        /// </summary>
        private static readonly DateTime dt1951 = new DateTime(1951, 1, 1);

        #endregion

        #region Public Methods and Operators

        public static DateTime? FixACValue(this DateTime dt)
        {
            return dt < dt1951 ? (DateTime?)null : dt;
        }

        #endregion
    }
}