namespace EdugameCloud.Lti.API.BrainHoney
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The DLAP extensions.
    /// </summary>
    public static class DlapExtensions
    {
        /// <summary>
        /// The to parameters.
        /// </summary>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string[] ToParams(this string queryString)
        {
            return queryString.Split("?&=".ToCharArray(), StringSplitOptions.None);
        }

        /// <summary>
        /// The to long.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long ToLong(this string str)
        {
            long result;
            return long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? result : 0;
        }

        /// <summary>
        /// The to integer.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ToInt(this string str)
        {
            int result;
            return int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ? result : 0;
        }
    }
}
