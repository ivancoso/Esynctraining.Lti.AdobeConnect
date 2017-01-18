namespace Esynctraining.AC.Provider.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The string extensions.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// The date time xml format.
        /// </summary>
        private const string DateTimeXmlFormat = @"yyyy-MM-dd\THH:mm:ss.fffzzz";

        /// <summary>
        /// The date time xml format local.
        /// </summary>
        private const string DateTimeXmlFormatLocal = @"yyyy-MM-dd\THH:mm:ss";

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

        public static long ParseLongWithDefault(this string stringValue, long defaultValue)
        {
            long result;

            return (!string.IsNullOrWhiteSpace(stringValue) && long.TryParse(stringValue, out result))
                ? result
                : defaultValue;
        }


        /// <summary>
        /// The append paging if needed.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AppendPagingIfNeeded(this string parameters, int startIndex, int limit)
        {
            if (startIndex != 0)
            {
                parameters += "&filter-start=" + startIndex;
            }

            if (limit != 0)
            {
                parameters += "&filter-rows=" + limit;
            }

            return parameters;
        }

        public static string AppendFilter(this string parameters, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return parameters;
            }
            
            return parameters + "&" + filter;
        }

        public static string AppendSortingIfNeeded(this string parameters, string propertySortBy, SortOrder order)
        {
            if (order == SortOrder.Unspecified)
                return parameters;
            if (string.IsNullOrWhiteSpace(propertySortBy))
                throw new ArgumentException("Non-empty property name expected", nameof(propertySortBy));

            //sort-name=asc 
            //sort-date=desc
            parameters += string.Format("&sort-{0}={1}", propertySortBy, order == SortOrder.Ascending ? "asc" : "desc");

            return parameters;
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
            if (string.IsNullOrWhiteSpace(stringValue))
                return defaultValue;

            DateTime result;

            return DateTime.TryParseExact(
                                stringValue,
                                DateTimeXmlFormat,
                                DateTimeFormatInfo.InvariantInfo,
                                DateTimeStyles.AdjustToUniversal,
                                out result)
                                ? result
                       : defaultValue;
        }

        /// <summary>
        /// The parse date time local.
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
        public static DateTime ParseDateTimeLocal(this string stringValue, DateTime defaultValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                return defaultValue;

            DateTime result;

            return DateTime.TryParseExact(
                                stringValue.Substring(0, DateTimeXmlFormatLocal.Length - DateTimeXmlFormatLocal.Count(c => c == '\\')),
                                DateTimeXmlFormatLocal,
                                DateTimeFormatInfo.InvariantInfo,
                                DateTimeStyles.None,
                                out result)
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
