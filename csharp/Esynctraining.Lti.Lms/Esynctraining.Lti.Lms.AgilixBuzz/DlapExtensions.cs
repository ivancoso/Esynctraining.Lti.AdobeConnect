using System;
using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.AgilixBuzz
{
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

        public static Dictionary<string, string> ToDictionary(this string queryString)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] parameters = queryString.Split("?&=".ToCharArray(), StringSplitOptions.None);
            for (int index = 0; index + 1 < parameters.Length; index += 2)
            {
                dictionary.Add(parameters[index], parameters[index + 1]);
            }
            return dictionary;
        }

    }
}
