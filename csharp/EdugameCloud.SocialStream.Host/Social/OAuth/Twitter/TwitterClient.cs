namespace EdugameCloud.MVC.Social.OAuth.Twitter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    using DotNetOpenAuth.AspNet.Clients;
    using DotNetOpenAuth.Messaging;

    using EdugameCloud.MVC.Social.OAuth.Instagram;

    using Newtonsoft.Json;

    /// <summary>
    /// The Twitter client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TwitterClient : OAuth2Client
    {
        ////https://dev.twitter.com/docs/auth/implementing-sign-twitter
        ////http://www.codeproject.com/Articles/247336/Twitter-OAuth-authentication-using-Net
        
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
        /// The user data cache.
        /// </summary>
        private static readonly Dictionary<string, TwitterRequestTokenResponse> userDataCache = new Dictionary<string, TwitterRequestTokenResponse>();

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
            var oauthCallback = returnUrl.AbsoluteUri.Replace("localhost:2345", "app.edugamecloud.com");
            const string OauthVersion = "1.0";
            var oauthConsumerKey = this.appId;
            var oauthConsumerSecret = this.appSecret;
            const string OauthSignatureMethod = "HMAC-SHA1";
            var oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);

            const string BaseFormat = "oauth_nonce={0}&oauth_callback={1}&oauth_signature_method={2}&oauth_timestamp={3}&oauth_consumer_key={4}&oauth_version={5}";

            var baseString = string.Format(
                BaseFormat,
                oauthNonce,
                oauthCallback,
                OauthSignatureMethod,
                oauthTimestamp,
                oauthConsumerKey,
                OauthVersion);

            baseString = string.Concat("POST&", Uri.EscapeDataString(RequestTokenEndpoint), "&", Uri.EscapeDataString(baseString));

            var compositeKey = Uri.EscapeDataString(oauthConsumerSecret);

            string oauthSignature;
            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
            }

            const string HeaderFormat = "OAuth oauth_nonce=\"{0}\", oauth_callback=\"{1}\", oauth_signature_method=\"{2}\", " +
                                        "oauth_timestamp=\"{3}\", " +
                                        "oauth_consumer_key=\"{4}\", oauth_signature=\"{5}\", " +
                                        "oauth_version=\"{6}\"";

            var authHeader = string.Format(
                HeaderFormat,
                Uri.EscapeDataString(oauthNonce),
                Uri.EscapeDataString(oauthCallback),
                Uri.EscapeDataString(OauthSignatureMethod),
                Uri.EscapeDataString(oauthTimestamp),
                Uri.EscapeDataString(oauthConsumerKey),
                Uri.EscapeDataString(oauthSignature),
                Uri.EscapeDataString(OauthVersion));

            ServicePointManager.Expect100Continue = false;

            var request = (HttpWebRequest)WebRequest.Create(RequestTokenEndpoint);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var sr = new StreamReader(responseStream))
                            {
                                var data = sr.ReadToEnd();
                                var tokens =
                                    data.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                        .ToDictionary(
                                            x =>
                                            x.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First(),
                                            x =>
                                            x.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                .ElementAt(1));
                                var builder = new UriBuilder(AuthenticateTokenEndpoint);
                                var parameters = new Dictionary<string, string>
                                                     {
                                                         {
                                                             "oauth_token",
                                                             tokens["oauth_token"]
                                                         },
                                                     };

                                foreach (var key in parameters.Keys)
                                {
                                    builder.AppendQueryArgument(key, parameters[key]);
                                }

                                return builder.Uri;
                            }
                        }

                        return null;
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response.GetResponseStream();
                if (response != null)
                {
                    var reader = new StreamReader(response);
                    string line;
                    var result = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Append(line);
                    }

                    var error = result.ToString();
                    throw new ApplicationException(error);
                }

                throw new ApplicationException(ex.ToString());
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
            var cleanUrl = redirectUrl.IndexOf("&code=", StringComparison.Ordinal) > 0 ? redirectUrl.Substring(0, redirectUrl.IndexOf("&code=", StringComparison.Ordinal)) : redirectUrl;

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
                    var response = ex.Response.GetResponseStream();
                    if (response != null)
                    {
                        var reader = new StreamReader(response);
                        string line;
                        var result = new StringBuilder();
                        while ((line = reader.ReadLine()) != null)
                        {
                            result.Append(line);
                        }

                        var error = result.ToString();
                        throw new ApplicationException(error);
                    }

                    throw new ApplicationException(ex.ToString());
                }
            }
        }

        #endregion
    }
}