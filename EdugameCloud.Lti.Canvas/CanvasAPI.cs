namespace EdugameCloud.Lti.Canvas
{
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using RestSharp;
    using Castle.Core.Logging;
    using System;

    /// <summary>
    /// The course API.
    /// </summary>
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
            _logger = logger;
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
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/users/{0}/profile", userId),
                    Method.GET,
                    userToken);

                IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetUser] API:{0}. UserToken:{1}. UserId:{2}.", api, userToken, userId);
                throw;
            }
        }

        public void AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id),
                    Method.POST,
                    userToken);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(submission);

                // ReSharper disable once UnusedVariable
                var res = client.Execute(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetUser] API:{0}. UserToken:{1}. SubmissionId:{2}.", api, userToken, submission.id);
                throw;
            }
        }

        public AnnouncementDTO CreateAnnouncement(
            string api,
            string userToken,
            int courseid,
            string title,
            string message)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);
                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/discussion_topics", courseid),
                    Method.POST,
                    userToken);
                request.AddParameter("title", title);
                request.AddParameter("message", message);
                request.AddParameter("is_announcement", true);

                IRestResponse<AnnouncementDTO> response = client.Execute<AnnouncementDTO>(request);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetUser] API:{0}. UserToken:{1}. CourseId:{2}.", api, userToken, courseid);
                throw;
            }
        }

        #endregion

        #region Methods

        protected List<CanvasQuestionDTO> GetQuestionsForQuiz(string api, string userToken, int courseId, int quizId)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseId, quizId),
                    Method.GET,
                    userToken);
                request.AddParameter("per_page", 1000);
                IRestResponse<List<CanvasQuestionDTO>> response = client.Execute<List<CanvasQuestionDTO>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetQuestionsForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}.", api, userToken, courseId, quizId);
                throw;
            }
        }

        protected LmsCourseDTO GetCourse(string api, string userToken, int courseId)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}", courseId),
                    Method.GET,
                    userToken);

                IRestResponse<LmsCourseDTO> response = client.Execute<LmsCourseDTO>(request);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetCourse] API:{0}. UserToken:{1}. CourseId:{2}.", api, userToken, courseId);
                throw;
            }
        }

        protected CanvasFileDTO GetFile(string api, string userToken, string fileId)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/files/{0}", fileId),
                    Method.GET,
                    userToken);

                IRestResponse<CanvasFileDTO> response = client.Execute<CanvasFileDTO>(request);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetFile] API:{0}. UserToken:{1}. FileId:{2}.", api, userToken, fileId);
                throw;
            }
        }

        protected static void Validate(string api, string userToken)
        {
            if (string.IsNullOrWhiteSpace(api))
                throw new ArgumentException("Api can not be empty", "api");

            if (string.IsNullOrWhiteSpace(api))
                throw new ArgumentException("UserToken can not be empty", "userToken");
        }

        protected static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(string.Format("{0}{1}", HttpScheme.Https, api));
            return client;
        }

        protected static RestRequest CreateRequest(string api, string resource, Method method, string usertoken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + usertoken);
            return request;
        }

        #endregion

    }

}