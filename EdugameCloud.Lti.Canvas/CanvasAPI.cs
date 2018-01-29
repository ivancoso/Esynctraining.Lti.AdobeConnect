namespace EdugameCloud.Lti.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Esynctraining.Core.Logging;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.DTO;
    using RestSharp;

    // ReSharper disable once InconsistentNaming
    public class CanvasAPI : ILmsAPI, ICanvasAPI
    {
        protected readonly ILogger _logger;

        #region Static Fields

        /// <summary>
        /// The canvas roles.
        /// </summary>
        protected static readonly string[] CanvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

        #endregion

        public CanvasAPI(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Public Methods and Operators

        public bool IsTokenExpired(string api, string userToken)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    "/api/v1/users/self/profile",
                    Method.GET,
                    userToken);

                IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

                return response.StatusCode == HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.IsTokenExpired] API:{0}. UserToken:{1}.", api, userToken);
                throw;
            }
        }

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

        public LmsUserDTO GetUser(string api, string userToken, string userId)
        {
            IRestResponse<LmsUserDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/users/{0}/profile", userId),
                    Method.GET,
                    userToken);

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
        
        public AnnouncementDTO CreateAnnouncement(
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
                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/discussion_topics", courseId),
                    Method.POST,
                    userToken);
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

        protected static RestRequest CreateRequest(string api, string resource, Method method, string userToken)
        {
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