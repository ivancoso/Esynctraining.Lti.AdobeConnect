namespace EdugameCloud.Lti.API.Canvas
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.BrainHoney;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Providers;

    using RestSharp;

    /// <summary>
    /// The course API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class DlapAPI
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        private string domainId;

        #region Static Fields

        /// <summary>
        /// The canvas roles.
        /// </summary>
        private static readonly string[] CanvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

        /// <summary>
        /// Initializes a new instance of the <see cref="DlapAPI"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DlapAPI(ApplicationSettingsProvider settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

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
        public void AnswerQuestionsForQuiz(string api, string usertoken, QuizSubmissionDTO submission)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/quiz_submissions/{0}/questions", submission.id), 
                Method.POST, 
                usertoken);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(submission);

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
        public List<QuizQuestionDTO> GetQuestionsForQuiz(string api, string usertoken, int courseid, int quizid)
        {
            var ret = new List<QuizQuestionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid), 
                Method.GET, 
                usertoken);

            IRestResponse<List<QuizQuestionDTO>> response = client.Execute<List<QuizQuestionDTO>>(request);

            ret.AddRange(response.Data);

            return ret;
        }

        /// <summary>
        /// The get course.
        /// </summary>
        /// <param name="api">
        /// The api.
        /// </param>
        /// <param name="usertoken">
        /// The usertoken.
        /// </param>
        /// <param name="courseid">
        /// The courseid.
        /// </param>
        /// <returns>
        /// The <see cref="CourseDTO"/>.
        /// </returns>
        public CourseDTO GetCourse(string api, string usertoken, int courseid)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}", courseid),
                Method.GET,
                usertoken);

            IRestResponse<CourseDTO> response = client.Execute<CourseDTO>(request);

            return response.Data;
        }

        /// <summary>
        /// The get quizzes for course.
        /// </summary>
        /// <param name="detailed">
        /// The detailed.
        /// </param>
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
        /// The <see cref="List{QuizDTO}"/>.
        /// </returns>
        public IEnumerable<LmsQuizDTO> GetQuizzesForCourse(bool detailed, string api, string usertoken, int courseid)
        {
            var ret = new List<LmsQuizDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes", courseid), 
                Method.GET, 
                usertoken);

            IRestResponse<List<LmsQuizDTO>> response = client.Execute<List<LmsQuizDTO>>(request);

            if (detailed)
            {
                foreach (LmsQuizDTO q in response.Data)
                {
                    q.questions = GetQuestionsForQuiz(api, usertoken, courseid, q.id).ToArray();
                }
            }

            ret.AddRange(response.Data);

            return ret;
        }

        /// <summary>
        /// The get quiz by id.
        /// </summary>
        /// <param name="api">
        /// The api.
        /// </param>
        /// <param name="usertoken">
        /// The usertoken.
        /// </param>
        /// <param name="courseid">
        /// The courseid.
        /// </param>
        /// <param name="quizid">
        /// The quizid.
        /// </param>
        /// <returns>
        /// The <see cref="LmsQuizDTO"/>.
        /// </returns>
        public LmsQuizDTO GetQuizById(string api, string usertoken, int courseid, string quizid)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}", courseid, quizid),
                Method.GET,
                usertoken);

            IRestResponse<LmsQuizDTO> response = client.Execute<LmsQuizDTO>(request);

            response.Data.questions = GetQuestionsForQuiz(api, usertoken, courseid, response.Data.id).ToArray();

            return response.Data;
        }

        /// <summary>
        /// The get submission for quiz.
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
        /// The <see cref="List{QuizSubmissionDTO}"/>.
        /// </returns>
        public List<QuizSubmissionDTO> GetSubmissionForQuiz(
            string api, 
            string usertoken, 
            int courseid, 
            int quizid)
        {
            var ret = new List<QuizSubmissionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions", courseid, quizid), 
                Method.POST, 
                usertoken);

            IRestResponse<QuizSubmissionResultDTO> response = client.Execute<QuizSubmissionResultDTO>(request);

            ret.AddRange(response.Data.quiz_submissions);

            return ret;
        }

        /// <summary>
        /// The get users for course.
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
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public static List<LmsUserDTO> GetUsersForCourse(string api, string usertoken, int courseid)
        {
            var ret = new List<LmsUserDTO>();
            var client = CreateRestClient(api);

            foreach (string role in CanvasRoles)
            {
                RestRequest request = CreateRequest(
                    api, 
                    string.Format("/api/v1/courses/{0}/users", courseid), 
                    Method.GET, 
                    usertoken);
                request.AddParameter("enrollment_type", role);

                IRestResponse<List<LmsUserDTO>> response = client.Execute<List<LmsUserDTO>>(request);

                List<LmsUserDTO> us = response.Data;
                us.ForEach(
                    u =>
                        {
                            u.canvas_role = role;
                            AddMoreDetailsForUser(api, usertoken, u);
                        });

                ret.AddRange(us);
            }

            return ret;
        }

        /// <summary>
        /// The return submission for quiz.
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
        /// <param name="submission">
        /// The submission.
        /// </param>
        public static void ReturnSubmissionForQuiz(
            string api, 
            string usertoken, 
            int courseid, 
            QuizSubmissionDTO submission)
        {
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api,
                string.Format("/api/v1/courses/{0}/quizzes/{1}/submissions/{2}/complete", courseid, submission.quiz_id, submission.id),
                Method.POST,
                usertoken);
            request.AddParameter("attempt", submission.attempt);
            request.AddParameter("validation_token", submission.validation_token);

            client.Execute(request);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="lmsUser">
        /// The lms User.
        /// </param>
        /// <returns>
        /// The <see cref="RestClient"/>.
        /// </returns>
        public Session BeginBatch(LmsUser lmsUser)
        {
            var lmsDomain = lmsUser.CompanyLms.LmsDomain;
            var session = new Session("EduGameCloud", (string)this.settings.BrainHoneyApiUrl) { Verbose = true };
            var userPrefix = lmsDomain.ToLower().Replace(".brainhoney.com", string.Empty).Replace("www.", string.Empty);

            XElement result = session.Login(userPrefix, lmsUser.Username, lmsUser.Password);
            if (!Session.IsSuccess(result))
            {
                this.logger.Error("Unable to login: " + Session.GetMessage(result));
                return null;
            }

            session.DomainId = result.XPathEvaluate("string(user/@domainid)").ToString();

            return session;
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
        private static RestRequest CreateRequest(string api, string resource, Method method, string usertoken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + usertoken);
            return request;
        }

        #endregion
    }
}