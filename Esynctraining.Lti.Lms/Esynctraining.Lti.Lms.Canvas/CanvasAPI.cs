using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using RestSharp;

namespace Esynctraining.Lti.Lms.Canvas
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

        public async Task<OAuthTokenResponse> RequestToken(string basePath, string oAuthId, string oAuthKey, string code, string lmsDomain)
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
            OAuthTokenResponse responseToken = _jsonDeserializer.JsonDeserialize<OAuthTokenResponse>(resp);
            return responseToken;

        }

        public async Task<string> RequestTokenByRefreshToken(RefreshTokenParamsDto refreshTokenParams, string lmsDomain)
        {
            IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            pairs.Add(new KeyValuePair<string, string>("client_id", refreshTokenParams.OAuthId));
            pairs.Add(new KeyValuePair<string, string>("client_secret", refreshTokenParams.OAuthKey));
            pairs.Add(new KeyValuePair<string, string>("refresh_token", refreshTokenParams.RefreshToken));

            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.PostAsync(
                $"https://{lmsDomain}/login/oauth2/token", new FormUrlEncodedContent(pairs));

            var resp = await httpResponseMessage.Content.ReadAsStringAsync();
            OAuthTokenResponse responseToken = _jsonDeserializer.JsonDeserialize<OAuthTokenResponse>(resp);
            return responseToken.access_token;
        }
        
        public async Task<bool> IsTokenExpired(string api, string userToken)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);
                var request = new RestRequest("/api/v1/users/self/profile", Method.GET);
                request.AddHeader("Authorization", "Bearer " + userToken);

                IRestResponse<LmsUserDTO> response = await client.ExecuteGetTaskAsync<LmsUserDTO>(request);
                return response.StatusCode == HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.IsTokenExpired] API:{0}. UserToken:{1}.", api, userToken);
                throw;
            }
        }

        // commented code migrated from AC LTI version
        //public void AddMoreDetailsForUser(string api, string userToken, LmsUserDTO user)
        //{
        //    try
        //    {
        //        Validate(api, userToken);

        //        LmsUserDTO canvasProfile = GetUser(api, userToken, user.id);

        //        if (canvasProfile != null)
        //        {
        //            user.primary_email = canvasProfile.primary_email;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorFormat(ex, "[CanvasAPI.AddMoreDetailsForUser] API:{0}. UserToken:{1}.", api, userToken);
        //        throw;
        //    }
        //}

        public async Task<LmsUserDTO> GetUser(string api, string userToken, string userId)
        {
            IRestResponse<LmsUserDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = await CreateRequest(
                    api,
                    string.Format("/api/v1/users/{0}/profile", userId),
                    Method.GET,
                    userToken); //todo: refreshParams if needed

                response = client.Execute<LmsUserDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetUser] API:{0}. UserToken:{1}. UserId:{2}.", api, userToken, userId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetUser] API:{0}. UserToken:{1}. UserId:{2}. {3}",
                    api, userToken, userId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetUser] Canvas returns '{0}'", response.StatusDescription));
            }

            return response.Data;
        }

        public async Task<AnnouncementDTO> CreateAnnouncement(
            string api,
            string userToken,
            int courseId,
            string title,
            string message)
        {
            IRestResponse<AnnouncementDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);
                RestRequest request = await CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/discussion_topics", courseId),
                    Method.POST,
                    userToken); //todo: refreshParams if needed 
                request.AddParameter("title", title);
                request.AddParameter("message", message);
                request.AddParameter("is_announcement", true);

                response = client.Execute<AnnouncementDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.CreateAnnouncement] API:{0}. UserToken:{1}. CourseId:{2}.", api, userToken, courseId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.CreateAnnouncement] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                    api, userToken, courseId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.CreateAnnouncement] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
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

        protected async Task<RestRequest> CreateRequest(string apiDomain, string resource, Method method, string userToken, RefreshTokenParamsDto refreshTokenParams = null)
        {
            if (refreshTokenParams != null && await IsTokenExpired(apiDomain, userToken))
            {
                var newAccessToken = await RequestTokenByRefreshToken(refreshTokenParams, apiDomain);
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
}