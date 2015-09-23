namespace EdugameCloud.Lti.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Castle.Core.Logging;
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

        public void AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission)
        {
            IRestResponse response = null;
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

                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.AnswerQuestionsForQuiz] API:{0}. UserToken:{1}. SubmissionId:{2}.",
                    api, userToken, submission.id);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.AnswerQuestionsForQuiz] API:{0}. UserToken:{1}. SubmissionId:{2}. {3}",
                    api, userToken, submission.id, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.AnswerQuestionsForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
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

        protected List<CanvasQuestionDTO> GetQuestionsForQuiz(string api, string userToken, int courseId, int quizId)
        {
            IRestResponse<List<CanvasQuestionDTO>> response;
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
                response = client.Execute<List<CanvasQuestionDTO>>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetQuestionsForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}.", api, userToken, courseId, quizId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetQuestionsForQuiz] API:{0}. UserToken:{1}. CourseId:{2}. QuizId:{3}. {4}",
                    api, userToken, courseId, quizId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetQuestionsForQuiz] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
        }

        protected LmsCourseDTO GetCourse(string api, string userToken, int courseId)
        {
            IRestResponse<LmsCourseDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/courses/{0}", courseId),
                    Method.GET,
                    userToken);

                response = client.Execute<LmsCourseDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetCourse] API:{0}. UserToken:{1}. CourseId:{2}.", api, userToken, courseId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetCourse] API:{0}. UserToken:{1}. CourseId:{2}. {3}",
                    api, userToken, courseId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetCourse] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
        }

        protected CanvasFileDTO GetFile(string api, string userToken, string fileId)
        {
            IRestResponse<CanvasFileDTO> response;
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    string.Format("/api/v1/files/{0}", fileId),
                    Method.GET,
                    userToken);

                response = client.Execute<CanvasFileDTO>(request);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[CanvasAPI.GetFile] API:{0}. UserToken:{1}. FileId:{2}.", api, userToken, fileId);
                throw;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.ErrorFormat("[CanvasAPI.GetFile] API:{0}. UserToken:{1}. FileId:{2}. {3}",
                    api, userToken, fileId, BuildInformation(response));
                throw new InvalidOperationException(string.Format("[CanvasAPI.GetFile] Canvas returns '{0}'", response.StatusDescription));
            }
            return response.Data;
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