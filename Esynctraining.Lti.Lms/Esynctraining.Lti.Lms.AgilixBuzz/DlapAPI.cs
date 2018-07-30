using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    public sealed partial class DlapAPI : ILmsAPI, IAgilixBuzzApi
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly dynamic _settings;

        #endregion

        #region Constructors and Destructors

        public DlapAPI(ApplicationSettingsProvider settings, ILogger logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods and Operators

        internal Task<(Session session, string error)> BeginBatchAsync(Dictionary<string, object> licenseSettings)
        {
            if (licenseSettings != null)
            {
                string lmsDomain = licenseSettings[LmsLicenseSettingNames.LmsDomain].ToString();
                string userLogin = licenseSettings[LmsLicenseSettingNames.BuzzAdminUsername].ToString();
                string password = licenseSettings[LmsLicenseSettingNames.BuzzAdminPassword].ToString();
                return LoginAndCreateASessionAsync(lmsDomain, userLogin, password);
            }

            return Task.FromResult(((Session)null, "ASP.NET Session is expired"));
        }

        internal async Task<(Session session, string error)> LoginAndCreateASessionAsync(string lmsDomain, string userName, string password)
        {
            try
            {
                var session = new Session(_logger, "EduGameCloud", (string)_settings.AgilixBuzzApiUrl) { Verbose = true };
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
            Dictionary<string, object> licenseSettings,
            string courseid,
            object extraData)
        {
            Session session = extraData as Session;
            var result = new List<LmsUserDTO>();

            var (courseResult, error) = await LoginIfNecessaryAsync(
                session,
                s => s.GetAsync(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseid).ToDictionary()),
                licenseSettings);

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
                licenseSettings);

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
                if (!string.IsNullOrWhiteSpace(privileges) && user != null && IsEnrollmentActive(status))
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
                            Email = email,
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
            if (long.TryParse(privileges, out long privilegesVal))
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

        private async Task<(T, string error)> LoginIfNecessaryAsync<T>(Session session, Func<Session, Task<T>> action, Dictionary<string, object> licenseSettings)
        {
            string error = null;

            if (session == null)
            {
                (session, error) = await BeginBatchAsync(licenseSettings);
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
