namespace EdugameCloud.Lti.API.Canvas
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using NHibernate.Cfg;

    using RestSharp;

    /// <summary>
    /// The course API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class CourseAPI : ILmsAPI
    {
        #region Static Fields

        /// <summary>
        /// The canvas roles.
        /// </summary>
        private static readonly string[] CanvasRoles = { "Teacher", "Ta", "Student", "Observer", "Designer" };

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
        public static void AddMoreDetailsForUser(string api, string usertoken, LmsUserDTO user)
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
        public static void AnswerQuestionsForQuiz(string api, string usertoken, QuizSubmissionDTO submission)
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
        public static AnnouncementDTO CreateAnnouncement(
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
        public static List<QuizQuestionDTO> GetQuestionsForQuiz(string api, string usertoken, int courseid, int quizid)
        {
            var ret = new List<QuizQuestionDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes/{1}/questions", courseid, quizid), 
                Method.GET, 
                usertoken);
            request.AddParameter("per_page", 100);
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
        public static CourseDTO GetCourse(string api, string usertoken, int courseid)
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
        /// <param name="quizIds">
        /// The quiz Ids.
        /// </param>
        /// <param name="isSurvey">
        /// The is Survey.
        /// </param>
        /// <returns>
        /// The <see cref="List{QuizDTO}"/>.
        /// </returns>
        public static IEnumerable<LmsQuizDTO> GetQuizzesForCourse(
            bool detailed,
            string api,
            string usertoken,
            int courseid,
            IEnumerable<int> quizIds,
            bool isSurvey)
        {
            var ret = new List<LmsQuizDTO>();
            var client = CreateRestClient(api);

            RestRequest request = CreateRequest(
                api, 
                string.Format("/api/v1/courses/{0}/quizzes", courseid), 
                Method.GET, 
                usertoken);
            request.AddParameter("per_page", 100);
            IRestResponse<List<LmsQuizDTO>> response = client.Execute<List<LmsQuizDTO>>(request);

            if (quizIds != null)
            {
                response.Data = response.Data.Where(q => quizIds.Contains(q.id)).ToList();
            }
            response.Data =
                response.Data.Where(
                    q =>
                    isSurvey ? q.quiz_type.ToLower().Contains("survey") : (!q.quiz_type.ToLower().Contains("survey")))
                    .ToList();

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
        public static LmsQuizDTO GetQuizById(string api, string usertoken, int courseid, string quizid)
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
        public static List<QuizSubmissionDTO> GetSubmissionForQuiz(
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
                    string.Format("/api/v1/courses/{0}/users?enrollment_type={1}&per_page={2}", courseid, role, 100),
                    Method.GET, 
                    usertoken);

                IRestResponse<List<LmsUserDTO>> response = client.Execute<List<LmsUserDTO>>(request);

                List<LmsUserDTO> us = response.Data;
                us.ForEach(
                    u =>
                        {
                            u.lms_role = role;
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
        /// <param name="api">
        /// The API.
        /// </param>
        /// <returns>
        /// The <see cref="RestClient"/>.
        /// </returns>
        private static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(string.Format("https://{0}", api));
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
        private static RestRequest CreateRequest(string api, string resource, Method method, string usertoken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + usertoken);
            return request;
        }

        #endregion
    }
}