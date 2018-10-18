using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using RestSharp;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasAPI : ILmsAPI, ICanvasAPI
    {
        protected readonly ILogger _logger;
        private readonly IJsonDeserializer _jsonDeserializer;

        #region Static Fields

        /// <summary>
        /// The canvas roles.
        /// </summary>
        protected static readonly string[] CanvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

        #endregion

        public CanvasAPI(ILogger logger, IJsonDeserializer jsonDeserializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonDeserializer = jsonDeserializer ?? throw new ArgumentNullException(nameof(jsonDeserializer));
        }

        #region Public Methods and Operators

        public async Task<ResponseToken> RequestToken(string basePath, string oAuthId, string oAuthKey, string code, string lmsDomain)
        {
            IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            pairs.Add(new KeyValuePair<string, string>("client_id", oAuthId));
            pairs.Add(new KeyValuePair<string, string>("redirect_uri", basePath));
            pairs.Add(new KeyValuePair<string, string>("client_secret", oAuthKey));
            pairs.Add(new KeyValuePair<string, string>("code", code));


            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                $"https://{lmsDomain}/login/oauth2/token", new FormUrlEncodedContent(pairs));

            var resp = await httpResponseMessage.Content.ReadAsStringAsync();
            ResponseToken responseToken = _jsonDeserializer.JsonDeserialize<ResponseToken>(resp);
            return responseToken;

        }

        public async Task<string> RequestTokenByRefreshToken(string refreshToken, string oAuthId, string oAuthKey, string lmsDomain)
        {
            IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            pairs.Add(new KeyValuePair<string, string>("client_id", oAuthId));
            pairs.Add(new KeyValuePair<string, string>("client_secret", oAuthKey));
            pairs.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));

            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                $"https://{lmsDomain}/login/oauth2/token", new FormUrlEncodedContent(pairs));

            var resp = await httpResponseMessage.Content.ReadAsStringAsync();
            ResponseToken responseToken = _jsonDeserializer.JsonDeserialize<ResponseToken>(resp);
            return responseToken.access_token;
        }


        public bool IsTokenExpired(string api, string userToken)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);
                var request = new RestRequest("/api/v1/users/self/profile", Method.GET);
                request.AddHeader("Authorization", "Bearer " + userToken);

                IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);
                return response.StatusCode == HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.IsTokenExpired] API:{0}. UserToken:{1}.", api, userToken);
                throw;
            }
        }

        public string BuildAuthUrl(string returnUrl, string lmsDomain, string oAuthId, string session)
        {
            var builder1 = new UriBuilder(returnUrl);
            builder1.AppendQueryArgument("providerUrl", HttpScheme.Https + lmsDomain);
            returnUrl = builder1.Uri.AbsoluteUri;

            var builder2 = new UriBuilder(returnUrl);
            builder2.AppendQueryArgument("providerKey", session);
            returnUrl = builder2.Uri.AbsoluteUri;

            if (string.IsNullOrEmpty(oAuthId))
            {
                var message = "Invalid OAuth parameters. Application Id and Application Key cannot be empty.";
                throw new Exception(message);
            }

            Uri uri = new Uri(returnUrl);
            var baseUri =
                uri.GetComponents(
                    UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                    UriFormat.UriEscaped);
            var query = QueryHelpers.ParseQuery(uri.Query);
            var items = query.SelectMany(x => x.Value,
                (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            string parameterValue = Guid.NewGuid().ToString("N");
            var qb = new QueryBuilder(items);
            qb.Add("__sid__", parameterValue);
            qb.Add("__provider__", "canvas");
            var fullUri = baseUri + qb.ToQueryString();


            var builder = new UriBuilder($"https://{lmsDomain}/login/oauth2/auth");

            var parameters = new Dictionary<string, string>
                    {
                        {"client_id", oAuthId},
                        {"redirect_uri", fullUri},
                        {"response_type", "code"},
                        {
                            "state",
                            Convert.ToBase64String(
                                Encoding.ASCII.GetBytes($"{lmsDomain}&&&ru={fullUri}"))
                        }
                    };

            foreach (var key in parameters.Keys)
            {
                builder.AppendQueryArgument(key, parameters[key]);
            }

            return builder.Uri.AbsoluteUri;
        }

        #endregion

        #region Methods

        protected static void Validate(string api, string userToken)
        {
            if (string.IsNullOrWhiteSpace(api))
                throw new ArgumentException("Api can not be empty", nameof(api));

            if (string.IsNullOrWhiteSpace(userToken))
                throw new ArgumentException("UserToken can not be empty", nameof(userToken));
        }

        protected static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(string.Format("{0}{1}", HttpScheme.Https, api));
            return client;
        }

        protected async Task<RestRequest> CreateRequest(string apiDomain, string resource, Method method, string userToken, string oauthId, string oauthKey, string refreshToken)
        {
            if (IsTokenExpired(apiDomain, userToken))
            {
                var newAccessToken = await RequestTokenByRefreshToken(refreshToken, oauthId, oauthKey, apiDomain);
                userToken = newAccessToken;
            }

            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + userToken);
            return request;
        }

        protected static string BuildInformation(IRestResponse response)
        {
            return string.Format("[Response] StatusCode: {0}. Content: {1}. ResponseErrorException: {2}.",
                    response.StatusCode,
                    response.Content, 
                    response.ErrorException);
        }


        #endregion

    }

    public class ResponseToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }

}