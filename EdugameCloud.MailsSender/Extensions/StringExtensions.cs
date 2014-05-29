namespace EdugameCloud.MailSender.Extensions
{
    using System;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The trim end strings.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <param name="removingPatterns">
        /// The removing patterns.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TrimEndStrings(this string str, params string[] removingPatterns)
        {
            foreach (string item in removingPatterns)
            {
                if (str.EndsWith(item))
                {
                    str = str.Substring(0, str.LastIndexOf(item, StringComparison.Ordinal));
                    break; 
                }
            }

            return str;
        }
    }
}
