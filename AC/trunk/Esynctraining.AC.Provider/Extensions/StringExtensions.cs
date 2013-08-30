namespace Esynctraining.AC.Provider.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The date time xml format.
        /// </summary>
        private const string DateTimeXmlFormat = @"yyyy-MM-dd\THH:mm:ss.fffzzz";

        /// <summary>
        /// The parse integer with default.
        /// </summary>
        /// <param name="stringValue">
        /// The string value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ParseIntWithDefault(this string stringValue, int defaultValue)
        {
            int result;

            return (!string.IsNullOrWhiteSpace(stringValue) && int.TryParse(stringValue, out result))
                ? result
                : defaultValue;
        }

        /// <summary>
        /// The parse date time with default.
        /// </summary>
        /// <param name="stringValue">
        /// The string value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ParseDateTimeWithDefault(this string stringValue, DateTime defaultValue)
        {
            DateTime result;

            return (!string.IsNullOrWhiteSpace(stringValue)
                    && DateTime.TryParseExact(
                                stringValue,
                                DateTimeXmlFormat,
                                DateTimeFormatInfo.InvariantInfo,
                                DateTimeStyles.AdjustToUniversal,
                                out result))
                       ? result
                       : defaultValue;
        }

        /// <summary>
        /// The parse boolean with default.
        /// </summary>
        /// <param name="stringValue">
        /// The string value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ParseBoolWithDefault(this string stringValue, bool defaultValue)
        {
            bool result;

            return (!string.IsNullOrWhiteSpace(stringValue) && bool.TryParse(stringValue, out result))
                ? result
                : defaultValue;
        }
    }
}
