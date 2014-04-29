namespace EdugameCloud.MVC.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Text;

    using DotNetOpenAuth.AspNet.Clients;
    using DotNetOpenAuth.Messaging;

    using Newtonsoft.Json;

    /// <summary>
    /// The Instagram client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class InstagramClient : OAuth2Client
    {
        #region Constants

        #region Constants and Fields

        /// <summary>
        ///     The instagram client.
        /// </summary>
        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://api.instagram.com/oauth/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://api.instagram.com/oauth/access_token";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string UserEndpoint = "https://api.instagram.com/v1/users/self?access_token=";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        private static Dictionary<string, InstagramUser> userDataCache = new Dictionary<string, InstagramUser>();

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
        /// Initializes a new instance of the <see cref="InstagramClient"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public InstagramClient(string appId, string appSecret)
            : base("instagram")
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
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(AuthorizationEndpoint);
            var parameters = new Dictionary<string, string>
                                 {
                                     { "client_id", this.appId }, 
                                     { "redirect_uri", returnUrl.AbsoluteUri }, 
                                     { "response_type", "code" }, 
                                 };
            foreach (var key in parameters.Keys)
            {
                builder.AppendQueryArgument(key, parameters[key]);
            }

            return builder.Uri;
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
            if (!userDataCache.ContainsKey(accessToken))
            {
                InstagramUserData graphData;
                var request = WebRequest.Create(UserEndpoint + this.EscapeUriDataStringRfc3986(accessToken));
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {
                            var data = sr.ReadToEnd();
                            graphData = JsonConvert.DeserializeObject<InstagramUserData>(data);
                        }
                    }
                }

                userDataCache.Add(accessToken, graphData.data);
            }

            var user = userDataCache[accessToken];

            // this dictionary must contains 
            var userData = new Dictionary<string, string>
                               {
                                   { "id", user.id }, 
                                   { "username", user.username }, 
                                   { "name", user.full_name }, 
                                   { "image", user.profile_picture }, 
                                   { "bio", user.bio }, 
                                   { "website", user.website },
                               };

            userDataCache.Remove(accessToken);
            return userData;
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
                    var response = client.UploadValues(TokenEndpoint, "POST", parameters);
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
                        userDataCache.Add(graphData.access_token, graphData.user);
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