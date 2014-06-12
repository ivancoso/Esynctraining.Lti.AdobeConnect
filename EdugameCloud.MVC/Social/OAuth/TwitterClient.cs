namespace EdugameCloud.MVC.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    using DotNetOpenAuth.AspNet.Clients;
    using DotNetOpenAuth.Messaging;

    using Newtonsoft.Json;

    /// <summary>
    /// The Twitter client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TwitterClient : OAuth2Client
    {
        ///https://dev.twitter.com/docs/auth/implementing-sign-twitter
        /// //http://www.codeproject.com/Articles/247336/Twitter-OAuth-authentication-using-Net
        
        #region Constants

        #region Constants and Fields

        /// <summary>
        ///     The instagram client.
        /// </summary>
        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string RequestTokenEndpoint = "https://api.twitter.com//oauth/request_token";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string AuthenticateTokenEndpoint = "https://api.twitter.com/oauth/authenticate";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string ConvertTokenEndpoint = "https://api.twitter.com/oauth/access_token";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        private static Dictionary<string, TwitterRequestTokenResponse> userDataCache = new Dictionary<string, TwitterRequestTokenResponse>();

        #endregion

        #region Fields

        /// <summary>
        ///     The _app id.
        /// </summary>
        private readonly string appId;

        /// <summary>
        ///     The _app secret.
        /// </summary>
        private readonly string appSecret;

        #endregion

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterClient"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public TwitterClient(string appId, string appSecret)
            : base("twitter")
        {
            this.appId = appId;
            this.appSecret = appSecret;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get service login url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// An absolute URI.
        /// </returns>
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var oauth_callback = returnUrl.AbsoluteUri.Replace("localhost:2345", "app.edugamecloud.com");
            var resource_url = RequestTokenEndpoint;
            var oauth_version = "1.0";
            var oauth_consumer_key = appId;
            var oauth_consumer_secret = appSecret;
            var oauth_signature_method = "HMAC-SHA1";
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            var baseFormat = "oauth_nonce={0}&oauth_callback={1}&oauth_signature_method={2}&oauth_timestamp={3}&oauth_consumer_key={4}&oauth_version={5}";

            var baseString = string.Format(
                baseFormat,
                oauth_nonce,
                oauth_callback,
                oauth_signature_method,
                oauth_timestamp,
                oauth_consumer_key,
                oauth_version);

            baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = Uri.EscapeDataString(oauth_consumer_secret);

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_callback=\"{1}\", oauth_signature_method=\"{2}\", " +
                   "oauth_timestamp=\"{3}\", " +
                   "oauth_consumer_key=\"{4}\", oauth_signature=\"{5}\", " +
                   "oauth_version=\"{6}\"";

            var authHeader = string.Format(
                headerFormat,
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_callback),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_version));

            ServicePointManager.Expect100Continue = false;

            var request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            var data = sr.ReadToEnd();
                            var tokens =
                                data.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                    .ToDictionary(x => x.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First(), x => x.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ElementAt(1));
                            var builder = new UriBuilder(AuthenticateTokenEndpoint);
                            var parameters = new Dictionary<string, string>
                                 {
                                     { "oauth_token", tokens["oauth_token"] }, 
                                 };

                            foreach (var key in parameters.Keys)
                            {
                                builder.AppendQueryArgument(key, parameters[key]);
                            }

                            return builder.Uri;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                string line;
                var result = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    result.Append(line);
                }

                var error = result.ToString();

                throw new ApplicationException(error);
            }
        }

        /// <summary>
        /// The get user data.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>
        /// A dictionary of profile data.
        /// </returns>
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
//            if (!userDataCache.ContainsKey(accessToken))
//            {
//                InstagramUserData graphData;
//                var request = WebRequest.Create(UserEndpoint + this.EscapeUriDataStringRfc3986(accessToken));
//                using (var response = request.GetResponse())
//                {
//                    using (var responseStream = response.GetResponseStream())
//                    {
//                        using (var sr = new StreamReader(responseStream))
//                        {
//                            var data = sr.ReadToEnd();
//                            graphData = JsonConvert.DeserializeObject<InstagramUserData>(data);
//                        }
//                    }
//                }
//
//                userDataCache.Add(accessToken, graphData.data);
//            }
//
//            var user = userDataCache[accessToken];
//
//            // this dictionary must contains 
//            var userData = new Dictionary<string, string>
//                               {
//                                   { "id", user.id }, 
//                                   { "username", user.username }, 
//                                   { "name", user.full_name }, 
//                                   { "image", user.profile_picture }, 
//                                   { "bio", user.bio }, 
//                                   { "website", user.website },
//                               };
//
//            userDataCache.Remove(accessToken);
//            return userData;

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Obtains an access token given an authorization code and callback URL.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="authorizationCode">
        /// The authorization code.
        /// </param>
        /// <returns>
        /// The access token.
        /// </returns>
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var redirectUrl = returnUrl.AbsoluteUri;
            var cleanUrl = redirectUrl.IndexOf("&code=") > 0 ? redirectUrl.Substring(0, redirectUrl.IndexOf("&code=")) : redirectUrl;

            var parameters = new NameValueCollection
                                 {
                                     { "client_id", this.appId }, 
                                     { "redirect_uri",  cleanUrl }, 
                                     { "client_secret", this.appSecret }, 
                                     { "code", authorizationCode }, 
                                     { "grant_type", "authorization_code" }, 
                                 };

            using (var client = new WebClient())
            {
                try
                {
                    var response = client.UploadValues(ConvertTokenEndpoint, "POST", parameters);
                    var data = Encoding.Default.GetString(response);
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }

                    var graphData = JsonConvert.DeserializeObject<InstagramAuthResponse>(data);

                    if (graphData == null)
                    {
                        return null;
                    }

                    if (!userDataCache.ContainsKey(graphData.access_token))
                    {
                        userDataCache.Add(graphData.access_token, null);
                    }

                    return graphData.access_token;
                }
                catch (WebException ex)
                {
                    var reader = new StreamReader(ex.Response.GetResponseStream());
                    string line;
                    var result = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Append(line);
                    }

                    var error = result.ToString();
                    
                    throw new ApplicationException(error);
                }
            }
        }

        /// <summary>
        /// Converts any % encoded values in the URL to uppercase.
        /// </summary>
        /// <param name="url">
        /// The URL string to normalize
        /// </param>
        /// <returns>
        /// The normalized url
        /// </returns>
        /// <example>
        /// NormalizeHexEncoding("Login.aspx?ReturnUrl=%2fAccount%2fManage.aspx") returns
        ///     "Login.aspx?ReturnUrl=%2FAccount%2FManage.aspx"
        /// </example>
        /// <remarks>
        /// There is an issue in Facebook whereby it will rejects the redirect_uri value if
        ///     the url contains lowercase % encoded values.
        /// </remarks>
        private static string NormalizeHexEncoding(string url)
        {
            char[] chars = url.ToCharArray();
            for (int i = 0; i < chars.Length - 2; i++)
            {
                if (chars[i] == '%')
                {
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    chars[i + 2] = char.ToUpperInvariant(chars[i + 2]);
                    i += 2;
                }
            }

            return new string(chars);
        }

        /// <summary>
        /// The escape uri data string rfc 3986.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EscapeUriDataStringRfc3986(string value)
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