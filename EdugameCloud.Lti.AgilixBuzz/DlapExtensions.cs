namespace EdugameCloud.Lti.AgilixBuzz
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

    }
}
