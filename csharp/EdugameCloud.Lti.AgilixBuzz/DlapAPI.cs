﻿namespace EdugameCloud.Lti.AgilixBuzz
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Esynctraining.Core.Logging;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AgilixBuzz;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using System.Threading.Tasks;

    // ReSharper disable once InconsistentNaming
    public sealed partial class DlapAPI : ILmsAPI, IAgilixBuzzApi
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

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
            _logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        internal Task<(Session session, string error)> BeginBatchAsync(ILmsLicense lmsCompany)
        {
            var lmsUser = lmsCompany.AdminUser;

            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.LmsCompany.LmsDomain;
                return LoginAndCreateASessionAsync(lmsDomain, lmsUser.Username, lmsUser.Password);
            }

            return Task.FromResult(((Session)null, "ASP.NET Session is expired"));
        }
        
        /// <summary>
        /// The login and create a session.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="Session"/>.
        /// </returns>
        internal async Task<(Session session, string error)> LoginAndCreateASessionAsync(string lmsDomain, string userName, string password)
        {
            try
            {
                var session = new Session(_logger, "EduGameCloud", (string)this.settings.AgilixBuzzApiUrl) { Verbose = true };
                string userPrefix = lmsDomain.ToLower()
                    .Replace(".agilixbuzz.com", string.Empty)
                    .Replace("www.", string.Empty);

                XElement result = await session.LoginAsync(userPrefix, userName, password);
                if (!Session.IsSuccess(result))
                {
                    var error = "Unable to login: " + Session.GetMessage(result);

                    _logger.Error(error);

                    return (null, error);
                }

                session.DomainId = result.XPathEvaluate("string(user/@domainid)").ToString();

                return (session, null);
            }
            catch (Exception ex)
            {
                _logger.Error("EdugameCloud.Lti.AgilixBuzz.DlapAPI.LoginAndCreateASession", ex);

                return (null, ex.Message);
            }
        }

        public async Task<(bool result, string error)> LoginAndCheckSessionAsync(string lmsDomain, string userName, string password)
        {
            var (session, error) = await LoginAndCreateASessionAsync(lmsDomain, userName, password);
            return (session != null, error);
        }
        
        public async Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(
            ILmsLicense company, 
            int courseid, 
            object extraData)
        {
            Session session = extraData as Session;
            var result = new List<LmsUserDTO>();

            var (courseResult, error) = await this.LoginIfNecessaryAsync(
                session,
                s => s.GetAsync(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseid).ToDictionary()),
                company);

            if (courseResult == null)
            {
                error = error ?? "DLAP. Unable to retrive result from API";

                return (result, error);
            }

            if (!Session.IsSuccess(courseResult))
            {
                error = "DLAP. Unable to create course: " + Session.GetMessage(courseResult);
                _logger.Error(error);
            }

            string domainId = courseResult.XPathSelectElement("course").XPathEvaluate("string(@domainid)").ToString();

            XElement enrollmentsResult;
            (enrollmentsResult, error) = await this.LoginIfNecessaryAsync(
                session, 
                s => s.GetAsync(Commands.Enrollments.List, string.Format(Parameters.Enrollments.List, domainId, courseid).ToDictionary()), 
                company);

            if (enrollmentsResult == null)
            {
                error = error ?? "DLAP. Unable to retrive result from API";
                return (result, error);
            }

            if (!Session.IsSuccess(enrollmentsResult))
            {
                error = "DLAP. Unable to create user: " + Session.GetMessage(enrollmentsResult);
                _logger.Error(error);
            }

            IEnumerable<XElement> enrollments = enrollmentsResult.XPathSelectElements("/enrollments/enrollment");
            foreach (XElement enrollment in enrollments)
            {
                string privileges = enrollment.XPathEvaluate("string(@privileges)").ToString();
                string status = enrollment.XPathEvaluate("string(@status)").ToString();
                XElement user = enrollment.XPathSelectElement("user");
                if (!string.IsNullOrWhiteSpace(privileges) && user != null && this.IsEnrollmentActive(status))
                {
                    var role = ProcessRole(privileges);
                    string userId = user.XPathEvaluate("string(@id)").ToString();
                    string firstName = user.XPathEvaluate("string(@firstname)").ToString();
                    string lastName = user.XPathEvaluate("string(@lastname)").ToString();
                    string userName = user.XPathEvaluate("string(@username)").ToString();
                    string email = user.XPathEvaluate("string(@email)").ToString();
                    result.Add(
                        new LmsUserDTO
                            {
                                LmsRole = role, 
                                PrimaryEmail = email, 
                                Login = userName, 
                                Id = userId, 
                                Name = firstName + " " + lastName, 
                            });
                }
            }

            return (result, error);
        }

        #endregion

        #region Methods

        internal bool IsEnrollmentActive(string enrollmentStatus)
        {
            switch (enrollmentStatus)
            {
                case "4": ////Withdrawn
                case "5": ////WithdrawnFailed
                case "6": ////Transfered
                case "9": ////Suspended
                case "10": ////Inactive
                    return false;
            }

            return true;
        }

        private static string ProcessRole(string privileges)
        {
            string role = Roles.Student;
            long privilegesVal;
            if (long.TryParse(privileges, out privilegesVal))
            {
                if (CheckRole(privilegesVal, RightsFlags.ControlCourse))
                {
                    role = Roles.Owner;
                }
                else if (CheckRole(privilegesVal, RightsFlags.ReadCourse)
                         && CheckRole(privilegesVal, RightsFlags.UpdateCourse)
                         && CheckRole(privilegesVal, RightsFlags.GradeAssignment)
                         && CheckRole(privilegesVal, RightsFlags.GradeForum)
                         && CheckRole(privilegesVal, RightsFlags.GradeExam)
                         && CheckRole(privilegesVal, RightsFlags.SetupGradebook)
                         && CheckRole(privilegesVal, RightsFlags.ReadGradebook)
                         && CheckRole(privilegesVal, RightsFlags.SubmitFinalGrade)
                         && CheckRole(privilegesVal, RightsFlags.ReadCourseFull))
                {
                    role = Roles.Teacher;
                }
                else if (CheckRole(privilegesVal, RightsFlags.ReadCourse)
                         && CheckRole(privilegesVal, RightsFlags.UpdateCourse)
                         && CheckRole(privilegesVal, RightsFlags.ReadCourseFull))
                {
                    role = Roles.Author;
                }
                else if (CheckRole(privilegesVal, RightsFlags.Participate)
                         && CheckRole(privilegesVal, RightsFlags.ReadCourse))
                {
                    role = Roles.Student;
                }
                else if (CheckRole(privilegesVal, RightsFlags.ReadCourse))
                {
                    role = Roles.Reader;
                }
            }

            role = Inflector.Capitalize(role);
            return role;
        }
        
        private static bool CheckRole(long privilegesVal, RightsFlags roleToCheck)
        {
            return ((RightsFlags)privilegesVal & roleToCheck) == roleToCheck;
        }

        /*

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
        } */
        
        private async Task<(T, string error)> LoginIfNecessaryAsync<T>(Session session, Func<Session, Task<T>> action, ILmsLicense lmsCompany)
        {
            string error = null;

            if (session == null)
            {
                (session, error) = await BeginBatchAsync(lmsCompany);
            }

            if (session != null)
            {
                return (await action(session), error);
            }

            return (default(T), error);
        }

        #endregion
    }

}