using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace EdugameCloud.Lti.Tests.FrontEnd
{
    public static class BltiBuilder
    {
        public static string Calculate( NameValueCollection form,
            string schema = "https",
            string port = "433", 
            string host = "app.edugamecloud.com", 
            string absolutePath = "/lti/canvas-login", 
            string sharedSecret = "4fbf260d-f8dc-4a06-a9cb-7a8bacee437d", 
            string httpMethod = "POST")
        {            
            // Generate the normalized URL
            // Note that the scheme and authority must be lowercase...HttpRequestBase.Url.Scheme and HttpRequestBase.Url.Host in C# are always lowercase
            string normalizedUrl = string.Format("{0}://{1}", schema, host);
            //if (
            //    !((schema == "http" && port == 80)
            //      || (schema == "https" && port == 443)
            //      || (schema == "https" && port == 80 && request.Url.Scheme == "http") // TRICK: using "X-Forwarded-Proto" proxy
            //      ))
            //{
            //    normalizedUrl += ":" + request.Url.Port;
            //}

            normalizedUrl += absolutePath;

            // Get a sorted list of all the form parameters except oauth_signature
            var parameters = new List<QueryParameter>();
            foreach (string key in form.AllKeys)
            {
                if (key != "oauth_signature")
                {
                    parameters.Add(new QueryParameter(key, form[key]));
                }
            }

            parameters.Sort(new QueryParameterComparer());
            string normalizedRequestParameters = parameters.NormalizeRequestParameters();

            //if (normalizedUrl.OAuthUrlEncode() == null)
            //{
            //    logger.Warn("[BltiProviderHelper] normalizedUrl.OAuthUrlEncode() == null");
            //    return false;
            //}
            //if (normalizedRequestParameters.OAuthUrlEncode() == null)
            //{
            //    logger.Warn("[BltiProviderHelper] normalizedRequestParameters.OAuthUrlEncode() == null");
            //    return false;
            //}

            // Create the signature base
            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&{1}&{2}",
                httpMethod
                , normalizedUrl.OAuthUrlEncode().Replace("%3a80", string.Empty)
                , normalizedRequestParameters.OAuthUrlEncode());
                        
            var hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", sharedSecret.OAuthUrlEncode(), string.Empty /*tokenSecret not used in BLTI*/));

            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureBase.ToString()));
            return Convert.ToBase64String(hashBytes);
        }
        
    }

    internal sealed class QueryParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public QueryParameter(string name, string value)
        {
            this.Name = name.OAuthUrlEncode();
            this.Value = value.OAuthUrlEncode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }

        #endregion
    }

    internal sealed class QueryParameterComparer : IComparer<QueryParameter>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.CompareOrdinal(x.Value, y.Value);
            }

            return string.CompareOrdinal(x.Name, y.Name);
        }

        #endregion
    }

    internal static class OAuthExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The OAuth url encode.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string OAuthUrlEncode(this string value)
        {
            // Per spec, all values are utf-8 encoded first
            byte[] buffer = Encoding.UTF8.GetBytes(value);

            var result = new StringBuilder();
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            foreach (char symbol in buffer)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + string.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// The normalize request parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string NormalizeRequestParameters(this IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter queryParameter = parameters[i];
                sb.AppendFormat("{0}={1}", queryParameter.Name, queryParameter.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }


        #endregion
    }

}
