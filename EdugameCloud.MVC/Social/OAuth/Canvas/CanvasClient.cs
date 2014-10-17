namespace EdugameCloud.MVC.Social.OAuth.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    using DotNetOpenAuth.AspNet.Clients;
    using DotNetOpenAuth.Messaging;
    using Esynctraining.Core.Extensions;

    using Newtonsoft.Json;

    /// <summary>
    /// The canvas client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class CanvasClient : OAuth2Client
    {
        #region Constants

        #region Constants and Fields

        /// <summary>
        ///     The return uri extension parameter.
        /// </summary>
        public const string ReturnUriExtensionQueryParameterName = "canvasUrl";

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://{0}/login/oauth2/auth";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://{0}/login/oauth2/token";

        #endregion

        #region Static Fields

        /// <summary>
        /// The user data cache.
        /// </summary>
        private static readonly Dictionary<string, CanvasUser> userDataCache = new Dictionary<string, CanvasUser>();

        /// <summary>
        /// The user canvas url cache.
        /// </summary>
        private static readonly Dictionary<string, string> userCanvasUrlCache = new Dictionary<string, string>();

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
        /// Initializes a new instance of the <see cref="CanvasClient"/> class. 
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public CanvasClient(string appId, string appSecret)
            : base("canvas")
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
            var collection = HttpUtility.ParseQueryString(returnUrl.Query);
            if (collection.HasKey(ReturnUriExtensionQueryParameterName))
            {
                var canvasUrl = collection[ReturnUriExtensionQueryParameterName];
                var returnUrlBuilder = new UriBuilder(returnUrl.AbsolutePath);
                foreach (var keyObject in collection.Keys)
                {
                    if (keyObject != null)
                    {
                        var key = keyObject.ToString();
                        if (!key.Equals(ReturnUriExtensionQueryParameterName, StringComparison.OrdinalIgnoreCase))
                        {
                            returnUrlBuilder.AppendQueryArgument(key, collection[key]);
                        }
                    }
                }

                var builder = new UriBuilder(string.Format(AuthorizationEndpoint, canvasUrl));
                var parameters = new Dictionary<string, string>
                                     {
                                         { "client_id", this.appId },
                                         { "redirect_uri", returnUrlBuilder.Uri.AbsoluteUri },
                                         { "response_type", "code" },
                                         { "scopes", "code" },
                                         { "state", Convert.ToBase64String(Encoding.ASCII.GetBytes(canvasUrl)) }
                                     };

                foreach (var key in parameters.Keys)
                {
                    builder.AppendQueryArgument(key, parameters[key]);
                }

                return builder.Uri;
            }

            return null;
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
            if (userCanvasUrlCache.ContainsKey(accessToken) && !userDataCache.ContainsKey(accessToken))
            {
                var canvasUrl = userCanvasUrlCache[accessToken];
                CanvasUser graphData = null;
                var request = WebRequest.Create(string.Format(AuthorizationEndpoint, canvasUrl));
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var sr = new StreamReader(responseStream))
                            {
                                var data = sr.ReadToEnd();
                                graphData = JsonConvert.DeserializeObject<CanvasUser>(data);
                            }
                        }
                    }
                }

                userDataCache.Add(accessToken, graphData);
            }

            if (userDataCache.ContainsKey(accessToken))
            {
                var user = userDataCache[accessToken];

                // this dictionary must contains 
                var userData = new Dictionary<string, string> { { "id", user.id }, { "name", user.name }, };

                userDataCache.Remove(accessToken);
                return userData;
            }

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
            var collection = HttpUtility.ParseQueryString(returnUrl.Query);
            if (collection.HasKey("state"))
            {
                var canvasUrl = Encoding.ASCII.GetString(Convert.FromBase64String(collection["state"]));
                
                var redirectUrl = returnUrl.AbsoluteUri;
                var cleanUrl = redirectUrl.IndexOf("&code=", StringComparison.Ordinal) > 0
                                   ? redirectUrl.Substring(0, redirectUrl.IndexOf("&code=", StringComparison.Ordinal))
                                   : redirectUrl;

                var parameters = new NameValueCollection
                                     {
                                         { "client_id", this.appId },
                                         { "redirect_uri", cleanUrl },
                                         { "client_secret", this.appSecret },
                                         { "code", authorizationCode },
                                     };

                using (var client = new WebClient())
                {
                    try
                    {
                        var response = client.UploadValues(string.Format(TokenEndpoint, canvasUrl), "POST", parameters);
                        var data = Encoding.Default.GetString(response);
                        if (string.IsNullOrEmpty(data))
                        {
                            return null;
                        }

                        var graphData = JsonConvert.DeserializeObject<CanvasAuthResponse>(data);

                        if (graphData == null)
                        {
                            return null;
                        }

                        if (!userCanvasUrlCache.ContainsKey(graphData.access_token))
                        {
                            userCanvasUrlCache.Add(graphData.access_token, canvasUrl);
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
            
            if (collection.HasKey("error"))
            {
                throw new ApplicationException(collection["error"]);
            }

            throw new ApplicationException("Invalid response from server");
        }

        #endregion
    }
}