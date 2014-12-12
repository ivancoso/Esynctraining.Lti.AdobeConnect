namespace EdugameCloud.Lti.Extensions
{
    using System;
    using EdugameCloud.Lti.Constants;

    /// <summary>
    /// The url string extension.
    /// </summary>
    public static class UrlStringExtension
    {
        /// <summary>
        /// The remove protocol.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveHttpProtocolAndTrailingSlash(this string url)
        {
            if (url.StartsWith(HttpScheme.Http, StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(HttpScheme.Http.Length);
            }

            if (url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(HttpScheme.Https.Length);
            }

            while (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        /// <summary>
        /// The is SSL.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool StartsWithProtocol(this string url)
        {
            return !string.IsNullOrWhiteSpace(url) && 
                (url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase) || url.StartsWith(HttpScheme.Http, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// The is SSL.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsSSL(this string url)
        {
            return !string.IsNullOrWhiteSpace(url) && url.StartsWith(HttpScheme.Https, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The add http protocol.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AddHttpProtocol(this string url, bool useSsl)
        {
            var domain = url.RemoveHttpProtocolAndTrailingSlash();
            if (useSsl)
            {
                return HttpScheme.Https + domain;
            }

            return HttpScheme.Http + domain;
        }
    }
}
