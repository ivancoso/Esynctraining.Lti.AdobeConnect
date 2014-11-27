﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UriBuilderExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The uri builder extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The uri builder extensions.
    /// </summary>
    public static class UriBuilderExtensions
    {
        #region Static Fields

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a name-value pair to the end of a given URL
        ///     as part of the querystring piece.  Prefixes a ? or &amp; before
        ///     first element as necessary.
        /// </summary>
        /// <param name="builder">
        /// The UriBuilder to add arguments to.
        /// </param>
        /// <param name="name">
        /// The name of the parameter to add.
        /// </param>
        /// <param name="value">
        /// The value of the argument.
        /// </param>
        /// <remarks>
        /// If the parameters to add match names of parameters that already are defined
        ///     in the query string, the existing ones are <i>not</i> replaced.
        /// </remarks>
        public static void AppendQueryArgument(this UriBuilder builder, string name, string value)
        {
            AppendQueryArgs(builder, new[] { new KeyValuePair<string, string>(name, value) });
        }

        #endregion

        #region Methods

        /// <summary>
        /// The append query args.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if (args != null && args.Count() > 0)
            {
                var sb = new StringBuilder(50 + (args.Count() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }

                sb.Append(CreateQueryString(args));

                builder.Query = sb.ToString();
            }
        }

        /// <summary>
        /// Concatenates a list of name-value pairs as key=value&amp;key=value,
        ///     taking care to properly encode each key and value for URL
        ///     transmission according to RFC 3986.  No ? is prefixed to the string.
        /// </summary>
        /// <param name="args">
        /// The dictionary of key/values to read from.
        /// </param>
        /// <returns>
        /// The formulated querystring style string.
        /// </returns>
        private static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            IList<KeyValuePair<string, string>> keyValuePairs = args as IList<KeyValuePair<string, string>>
                                                                ?? args.ToList();
            if (!keyValuePairs.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder(keyValuePairs.Count() * 10);

            foreach (var p in keyValuePairs)
            {
                sb.Append(EscapeUriDataStringRfc3986(p.Key));
                sb.Append('=');
                sb.Append(EscapeUriDataStringRfc3986(p.Value));
                sb.Append('&');
            }

            sb.Length--; // remove trailing &

            return sb.ToString();
        }

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">
        /// The value to escape.
        /// </param>
        /// <returns>
        /// The escaped value.
        /// </returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        ///     RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        ///     actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        ///     host actually having this configuration element present.
        /// </remarks>
        private static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            var escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

        #endregion
    }
}