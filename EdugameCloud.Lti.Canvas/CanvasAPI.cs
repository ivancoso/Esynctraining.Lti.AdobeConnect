namespace EdugameCloud.Lti.Canvas
{
    using System.Collections.Generic;
    using System.Net;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.DTO;
    using RestSharp;

    /// <summary>
    /// The course API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class CanvasAPI : ILmsAPI, ICanvasAPI
    {
        #region Static Fields

        /// <summary>
        /// The canvas roles.
        /// </summary>
        protected static readonly string[] CanvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Checks if token expired.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsTokenExpired(string api, string usertoken)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                "/api/v1/users/self/profile",
                Method.GET,
                usertoken);

            IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

            return response.StatusCode == HttpStatusCode.Unauthorized;
        }

        /// <summary>
        /// The add more details for user.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public void AddMoreDetailsForUser(string api, string usertoken, LmsUserDTO user)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/users/{0}/profile", user.id), 
                Method.GET, 
                usertoken);

            IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

            if (response.Data != null)
            {
                user.primary_email = response.Data.primary_email;
            }
        }

        /// <summary>
        /// The answer questions for quiz.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="submission">
        /// The submission.
        /// </param>
        public void AnswerQuestionsForQuiz(string api, string usertoken, CanvasQuizSubmissionDTO submission)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id), 
                Method.POST, 
                usertoken);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(submission);

            // ReSharper disable once UnusedVariable
            var res = client.Execute(request);
        }

        /// <summary>
        /// The create announcement.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="AnnouncementDTO"/>.
        /// </returns>
        public AnnouncementDTO CreateAnnouncement(
            string api, 
            string usertoken, 
            int courseid, 
            string title, 
            string message)
        {
            var client = CreateRestClient(api);
            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/discussion_topics", courseid), 
                Method.POST, 
                usertoken);
            request.AddParameter("title", title);
            request.AddParameter("message", message);
            request.AddParameter("is_announcement", true);

            IRestResponse<AnnouncementDTO> response = client.Execute<AnnouncementDTO>(request);

            return response.Data;
        }

        /// <summary>
        /// The get questions for quiz.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="quizid">
        /// The quiz id.
        /// </param>
        /// <returns>
        /// The <see cref="List{QuizQuestionDTO}"/>.
        /// </returns>
        public static List<CanvasQuestionDTO> GetQuestionsForQuiz(string api, string usertoken, int courseid, int quizid)
        {
            var ret = new List<CanvasQuestionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid), 
                Method.GET, 
                usertoken);
            request.AddParameter("per_page", 1000);
            IRestResponse<List<CanvasQuestionDTO>> response = client.Execute<List<CanvasQuestionDTO>>(request);

            ret.AddRange(response.Data);

            return ret;
        }

        /// <summary>
        /// The get course.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <returns>
        /// The <see cref="LmsCourseDTO"/>.
        /// </returns>
        public static LmsCourseDTO GetCourse(string api, string usertoken, int courseid)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}", courseid),
                Method.GET,
                usertoken);

            IRestResponse<LmsCourseDTO> response = client.Execute<LmsCourseDTO>(request);

            return response.Data;
        }

        /// <summary>
        /// The get file.
        /// </summary>
        /// <param name="api">
        /// The api.
        /// </param>
        /// <param name="usertoken">
        /// The usertoken.
        /// </param>
        /// <param name="fileid">
        /// The fileid.
        /// </param>
        /// <returns>
        /// The <see cref="CanvasFileDTO"/>.
        /// </returns>
        public static CanvasFileDTO GetFile(string api, string usertoken, string fileid)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/files/{0}", fileid),
                Method.GET,
                usertoken);

            IRestResponse<CanvasFileDTO> response = client.Execute<CanvasFileDTO>(request);

            return response.Data;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <returns>
        /// The <see cref="RestClient"/>.
        /// </returns>
        protected static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(string.Format("{0}{1}", HttpScheme.Https, api));
            return client;
        }

        /// <summary>
        /// The create request.
        /// </summary>
        /// <param name="api">
        /// The API.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="usertoken">
        /// The user token.
        /// </param>
        /// <returns>
        /// The <see cref="RestRequest"/>.
        /// </returns>
        // ReSharper disable once UnusedParameter.Local
        protected static RestRequest CreateRequest(string api, string resource, Method method, string usertoken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + usertoken);
            return request;
        }

        #endregion
    }
}