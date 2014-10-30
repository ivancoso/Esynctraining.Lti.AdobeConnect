namespace EdugameCloud.Lti.API.BrainHoney
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.SessionState;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    using RestSharp;

    /// <summary>
    ///     The course API.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class DlapAPI
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The settings.
        /// </summary>
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
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create rest client.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS User.
        /// </param>
        /// <returns>
        /// The <see cref="RestClient"/>.
        /// </returns>
        public Session BeginBatch(out string error, LmsUser lmsUser = null)
        {
            if (lmsUser == null)
            {
                HttpSessionState session = HttpContext.Current.With(x => x.Session);
                string companyKey = string.Format(LtiSessionKeys.CredentialsSessionKeyPattern, "brainhoney");
                if (session != null && session[companyKey] != null)
                {
                    var companyLms = session[companyKey] as CompanyLms;
                    lmsUser = companyLms.With(x => x.AdminUser);
                }
            }

            if (lmsUser != null)
            {
                string lmsDomain = lmsUser.CompanyLms.LmsDomain;
                var session = new Session("EduGameCloud", (string)this.settings.BrainHoneyApiUrl) { Verbose = true };
                string userPrefix = lmsDomain.ToLower()
                    .Replace(".brainhoney.com", string.Empty)
                    .Replace("www.", string.Empty);

                XElement result = session.Login(userPrefix, lmsUser.Username, lmsUser.Password);
                if (!Session.IsSuccess(result))
                {
                    error = "DLAP. Unable to login: " + Session.GetMessage(result);
                    this.logger.Error(error);
                    return null;
                }

                error = null;
                session.DomainId = result.XPathEvaluate("string(user/@domainid)").ToString();
                return session;
            }

            error = "ASP.NET Session is expired";
            return null;
        }

        /// <summary>
        /// The get users for course.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="courseid">
        /// The course id.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="List{LmsUserDTO}"/>.
        /// </returns>
        public List<LmsUserDTO> GetUsersForCourse(
            CompanyLms company, 
            int courseid, 
            out string error, 
            Session session = null)
        {
            var result = new List<LmsUserDTO>();
            XElement enrollmentsResult = this.LoginIfNecessary(
                session, 
                s =>
                s.Get(
                    Commands.Enrollments.List, 
                    string.Format(Parameters.Enrollments.List, s.DomainId, courseid).ToParams()), 
                out error);
            if (enrollmentsResult == null)
            {
                error = error ?? "DLAP. Unable to retrive result from API";
                return result;
            }

            if (!Session.IsSuccess(enrollmentsResult))
            {
                error = "DLAP. Unable to create user: " + Session.GetMessage(enrollmentsResult);
                this.logger.Error(error);
            }

            IEnumerable<XElement> enrollments = enrollmentsResult.XPathSelectElements("/enrollments/enrollment");
            foreach (XElement enrollment in enrollments)
            {
                string role = Roles.Student;
                string privileges = enrollment.XPathEvaluate("string(@privileges)").ToString();
                XElement user = enrollment.XPathSelectElement("user");
                if (!string.IsNullOrWhiteSpace(privileges) && user != null)
                {
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

                    string userId = user.XPathEvaluate("string(@id)").ToString();
                    string firstName = user.XPathEvaluate("string(@firstname)").ToString();
                    string lastName = user.XPathEvaluate("string(@lastname)").ToString();
                    string userName = user.XPathEvaluate("string(@username)").ToString();
                    string email = user.XPathEvaluate("string(@email)").ToString();
                    result.Add(
                        new LmsUserDTO
                            {
                                lms_role = role, 
                                primary_email = email, 
                                login_id = userName, 
                                id = userId, 
                                name = firstName + " " + lastName, 
                            });
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check role.
        /// </summary>
        /// <param name="privilegesVal">
        /// The privileges val.
        /// </param>
        /// <param name="roleToCheck">
        /// The role to check.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private void LoginIfNecessary(Session session, Action<Session> action)
        {
            string error = null;
            session = session ?? this.BeginBatch(out error);
            action(session);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(Session session, Func<Session, T> action)
        {
            string error = null;
            session = session ?? this.BeginBatch(out error);
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(Session session, Func<Session, T> action, out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error);
            if (session != null)
            {
                return action(session);
            }

            return default(T);
        }

        /// <summary>
        /// The login if necessary.
        /// </summary>
        /// <typeparam name="T">
        /// Any type
        /// </typeparam>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private T LoginIfNecessary<T>(Session session, Func<Session, Tuple<T, string>> action, out string error)
        {
            error = null;
            session = session ?? this.BeginBatch(out error);
            if (session != null)
            {
                Tuple<T, string> resTuple = action(session);
                error = resTuple.Item2;
                return resTuple.Item1;
            }

            error = error ?? "DLAP. Session is null";
            return default(T);
        }

        #endregion

        /// <summary>
        ///     The commands.
        /// </summary>
        protected class Commands
        {
            /// <summary>
            ///     The enrollments.
            /// </summary>
            public class Enrollments
            {
                #region Constants

                /// <summary>
                /// The list.
                /// </summary>
                public const string List = "listenrollments";

                #endregion
            }
        }

        /// <summary>
        /// The parameters.
        /// </summary>
        protected class Parameters
        {
            /// <summary>
            /// The enrollments.
            /// </summary>
            public class Enrollments
            {
                #region Constants

                /// <summary>
                /// The list.
                /// </summary>
                public const string List = "domainid={0}&limit=0&coursequery=%2Fid%3D{1}&select=user";

                #endregion
            }
        }

        /// <summary>
        /// The roles.
        /// </summary>
        protected class Roles
        {
            #region Constants

            /// <summary>
            /// The author.
            /// </summary>
            public const string Author = "author";

            /// <summary>
            /// The owner.
            /// </summary>
            public const string Owner = "owner";

            /// <summary>
            /// The reader.
            /// </summary>
            public const string Reader = "reader";

            /// <summary>
            /// The student.
            /// </summary>
            public const string Student = "student";

            /// <summary>
            /// The teacher.
            /// </summary>
            public const string Teacher = "teacher";

            #endregion
        }
    }
}