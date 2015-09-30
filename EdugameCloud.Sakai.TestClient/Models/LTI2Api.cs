using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EdugameCloud.Sakai.TestClient.Models
{
    public class LTI2Api
    {
        #region Public Methods and Operators
        
        public static Tuple<string, string> CreateSignedRequestAndGetResponse(
            SakaiParameters parameters)
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string ltiMessageType = parameters.LtiMessageType,
                   oauthNonce =
                       Convert.ToBase64String(
                           new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture))),
                   oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture),
                   oauthCallback = "about:blank",
                   key = parameters.ConsumerKey,
                   secret = parameters.SharedSecret,
                   url = parameters.ServiceUrl;

            string ltiVersion = parameters.LtiVersion;

            const string BaseFormat =
                "id={0}&"
                + "lti_message_type={1}&"
                + "lti_version={2}&"
                + "oauth_callback={3}&"
                + "oauth_consumer_key={4}&"
                + "oauth_nonce={5}&"
                + "oauth_signature_method={6}&"
                + "oauth_timestamp={7}&"
                + "oauth_version={8}";

            string baseString = string.Format(
                BaseFormat,
                Uri.EscapeDataString(parameters.lis_result_sourcedid),
                ltiMessageType,
                ltiVersion,
                Uri.EscapeDataString(oauthCallback),
                key,
                oauthNonce,
                parameters.OAuthSignatureMethod,
                oauthTimestamp,
                parameters.OAuthVersion);

            baseString = string.Concat("POST&", Uri.EscapeDataString(url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = Uri.EscapeDataString(secret) + "&";

            string oauthSignature;
            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
            }

            ServicePointManager.Expect100Continue = false;

            var request = (HttpWebRequest)WebRequest.Create(url);

            var pairs = new NameValueCollection
            {
                { "id", parameters.lis_result_sourcedid },
                { "lti_message_type", ltiMessageType },
                { "lti_version", ltiVersion },
                { "oauth_callback", oauthCallback },
                { "oauth_consumer_key", key },
                { "oauth_nonce", oauthNonce },
                { "oauth_signature", oauthSignature },
                { "oauth_signature_method", parameters.OAuthSignatureMethod },
                { "oauth_timestamp", oauthTimestamp },
                { "oauth_version", parameters.OAuthVersion }
            };

            var builder = new UriBuilder(url);

            foreach (string pkey in pairs.Keys)
            {
                builder.AppendQueryArgument(pkey, pairs[pkey]);
            }

            byte[] bytes = Encoding.UTF8.GetBytes(builder.Uri.Query.TrimStart("?".ToCharArray()));

            string resp;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Timeout = 5000;  // TODO: add timeout
            request.Referer = url;
            request.Host = new Uri(url).Host;
            request.ContentLength = bytes.Length;
            using (Stream requeststream = request.GetRequestStream())
            {
                requeststream.Write(bytes, 0, bytes.Length);
                requeststream.Close();
            }

            using (var webResponse = (HttpWebResponse)request.GetResponse())
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    resp = sr.ReadToEnd().Trim();
                    sr.Close();
                }

                webResponse.Close();
            }
            
            return Tuple.Create(builder.Uri.Query.TrimStart("?".ToCharArray()), resp);
        }

        #endregion
        
    }

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

        public static string AddQueryStringParameter(string url, string name, string value)
        {
            var builder = new UriBuilder(url);
            builder.AppendQueryArgument(name, value);
            return builder.Uri.AbsoluteUri;
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
            // ReSharper disable once PossibleMultipleEnumeration
            var argsCount = args.Count();
            if (args != null && argsCount > 0)
            {
                var sb = new StringBuilder(50 + (argsCount * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }

                // ReSharper disable once PossibleMultipleEnumeration
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
        /// The formulated query string style string.
        /// </returns>
        private static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            var keyValuePairs = args as IList<KeyValuePair<string, string>> ?? args.ToList();
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