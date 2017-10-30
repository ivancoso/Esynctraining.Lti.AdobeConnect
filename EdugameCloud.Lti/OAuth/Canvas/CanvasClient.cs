namespace EdugameCloud.Lti.OAuth.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    using DotNetOpenAuth.AspNet.Clients;
    using DotNetOpenAuth.Messaging;
    using Esynctraining.Core.Extensions;

    using Newtonsoft.Json;
    using EdugameCloud.HttpClient;
    using System.Net.Http;

    /// <summary>
    /// The canvas client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CanvasClient : OAuth2Client
    {
        #region Constants

        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "{0}/login/oauth2/auth";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "{0}/login/oauth2/token";

        /// <summary>
        /// The return url delimiter.
        /// </summary>
        private const string ReturnUrlDelimiter = "&&&ru=";

        /// <summary>
        /// The state query key.
        /// </summary>
        private const string StateQueryKey = "state";

        /// <summary>
        /// The return uri provider key parameter name.
        /// </summary>
        private const string ReturnUriProviderKeyParameterName = "providerKey";

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

        private static readonly HttpClientWrapper _httpClientWrapper = new HttpClientWrapper();
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
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentException(nameof(appId));
            if (string.IsNullOrEmpty(appSecret))
                throw new ArgumentException(nameof(appSecret));
            this.appId = appId;
            this.appSecret = appSecret;
        }

        #endregion

        #region Methods

        //public static string AddCanvasUrlToReturnUrl(string returnUrl, string canvasUrl)
        //{
        //    var builder = new UriBuilder(returnUrl);
        //    builder.AppendQueryArgument(Core.Utils.Constants.ReturnUriExtensionQueryParameterName, canvasUrl);
        //    return builder.Uri.AbsoluteUri;
        //}

        /// <summary>
        /// The add provider key to return url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AddProviderKeyToReturnUrl(string returnUrl, string providerKey)
        {
            var builder = new UriBuilder(returnUrl);
            builder.AppendQueryArgument(ReturnUriProviderKeyParameterName, providerKey);
            return builder.Uri.AbsoluteUri;
        }

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
            if (collection.HasKey(Core.Utils.Constants.ReturnUriExtensionQueryParameterName))
            {
                var canvasUrl = collection[Core.Utils.Constants.ReturnUriExtensionQueryParameterName];
                var returnUri = ClearReturnUrl(returnUrl, collection);
                var returnUrlFixed = returnUri.AbsoluteUri;

                var builder = new UriBuilder(string.Format(AuthorizationEndpoint, canvasUrl));
                var parameters = new Dictionary<string, string>
                                     {
                                         { "client_id", this.appId },
                                         { "redirect_uri", returnUrlFixed },
                                         { "response_type", "code" },
                                         { "state", Convert.ToBase64String(Encoding.ASCII.GetBytes(canvasUrl + ReturnUrlDelimiter  + returnUrlFixed)) }
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
            if (collection.HasKey(StateQueryKey))
            {
                var canvasUrlAndReturnUrl = Encoding.ASCII.GetString(Convert.FromBase64String(collection[StateQueryKey]));
                var urls = canvasUrlAndReturnUrl.Split(new[] { ReturnUrlDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                var canvasUrl = urls.FirstOrDefault();
                var redirectUrl = urls.ElementAtOrDefault(1).Return(x => x, returnUrl.AbsoluteUri);

                var parameters = new Dictionary<string, string>()
                {
                    { "grant_type", "authorization_code"}, //marked as required by Canvas but worked without it
                    { "client_id", this.appId },
                    { "redirect_uri", redirectUrl },
                    { "client_secret", this.appSecret },
                    { "code", authorizationCode },
                };

                try
                {
                    var canvasGetTokenUrl = string.Format(TokenEndpoint, canvasUrl);
                    var httpClientWrapper = new HttpClientWrapper();
                    var data = httpClientWrapper.PostValuesAsync(canvasGetTokenUrl, parameters).Result;

                    if (string.IsNullOrEmpty(data))
                    {
                        throw new Exception("data is empty");
                    }

                    var authResponse = JsonConvert.DeserializeObject<CanvasAuthResponse>(data);

                    if (authResponse == null)
                    {
                        throw new Exception("auth response == null " + data);
                    }

                    if (!userCanvasUrlCache.ContainsKey(authResponse.access_token))
                    {
                        userCanvasUrlCache.Add(authResponse.access_token, canvasUrl);
                    }

                    if (authResponse.user != null && !userDataCache.ContainsKey(authResponse.access_token))
                    {
                        userDataCache.Add(authResponse.access_token, authResponse.user);
                    }

                    return authResponse.access_token;
                }
                catch (HttpRequestException ex)
                {
                    throw new ApplicationException(ex.ToString());
                }
            }
            
            if (collection.HasKey("error"))
            {
                throw new ApplicationException(collection["error"]);
            }

            throw new ApplicationException("Invalid response from server");
        }

        /// <summary>
        /// The clear return url.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        private static Uri ClearReturnUrl(Uri returnUrl, NameValueCollection collection)
        {
            var returnUrlBuilder = new UriBuilder(returnUrl.GetLeftPart(UriPartial.Path));
            foreach (var keyObject in collection.Keys)
            {
                if (keyObject != null)
                {
                    var key = keyObject.ToString();
                    if (!key.Equals(Core.Utils.Constants.ReturnUriExtensionQueryParameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        returnUrlBuilder.AppendQueryArgument(key, collection[key]);
                    }
                }
            }

            return returnUrlBuilder.Uri;
        }

        #endregion
    }
}