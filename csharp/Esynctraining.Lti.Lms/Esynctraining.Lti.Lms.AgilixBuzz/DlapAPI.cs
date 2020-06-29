using Esynctraining.Core.Logging;
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
        private readonly ILogger _logger;
        private readonly BuzzApiClient _buzzClient;

        public DlapAPI(ILogger logger, BuzzApiClient buzzClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _buzzClient = buzzClient ?? throw new ArgumentNullException(nameof(buzzClient));
        }

        #region Public Methods and Operators
        public async Task<(List<LmsUserDTO> users, string error)> GetUsersForCourseAsync(
            Dictionary<string, object> licenseSettings,
            string courseid,
            object extraData)
        {
            var result = new List<LmsUserDTO>();
            if(_buzzClient.DomainId == null)
            {
                var loginResponse = await InitializeClient(licenseSettings);
                if(loginResponse == null || _buzzClient.DomainId == null)
                {
                    return (null, "Unable to login to Buzz API");
                }
            }
            var courseResult = await _buzzClient.GetAsync(Commands.Courses.GetOne, string.Format(Parameters.Courses.GetOne, courseid).ToDictionary());

            if (courseResult == null)
            {
                var error = "Unable to retrive result from Buzz API";
                return (result, error);
            }

            string domainId = courseResult.XPathSelectElement("course").XPathEvaluate("string(@domainid)").ToString();

            XElement enrollmentsResult = await _buzzClient.GetAsync(Commands.Enrollments.List, string.Format(Parameters.Enrollments.List, domainId, courseid).ToDictionary());

            if (enrollmentsResult == null)
            {
                var error = "Unable to retrive result from Buzz API";
                return (result, error);
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

            return (result, null);
        }

        public async Task<bool> LoginAndCheckSessionAsync(string lmsDomain, string userName, string password)
        {
            XElement result;
            try
            {
                var uri = new Uri(
                    lmsDomain.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? lmsDomain
                        : "http://" + lmsDomain
                );

                string userPrefix = uri.Host.ToLower()
                    .Replace(".agilixbuzz.com", string.Empty)
                    .Replace("www.", string.Empty);

                result = await _buzzClient.LoginAsync(userPrefix, userName, password); //sets DomainId internally
            }
            catch (Exception ex)
            {
                _logger.Error("EdugameCloud.Lti.AgilixBuzz.DlapAPI.LoginAndCreateASession", ex);

                return false;
            }

            return result != null;
        }

        public async Task<LmsUserDTO> GetUserAsync(Dictionary<string, object> licenseSettings,
            string lmsUserId,
            object extraData = null)
        {
            if (_buzzClient.DomainId == null)
            {
                var loginResponse = await InitializeClient(licenseSettings);
                if (loginResponse == null || _buzzClient.DomainId == null)
                {
                    return null;
                }
            }

            var userResult = await _buzzClient.GetAsync(Commands.Users.GetOne,
                    string.Format(Parameters.Users.GetOne, lmsUserId).ToDictionary());

            if (userResult != null)
            {
                var user = userResult.XPathSelectElement("user");
                var userId = user.XPathEvaluate("string(@id)").ToString();
                var email = user.XPathEvaluate("string(@email)").ToString();
                var userName = user.XPathEvaluate("string(@username)").ToString();

                return new LmsUserDTO
                {
                    Email = email,
                    Login = userName,
                    Id = userId,
                    Name = userName,
                };
            }

            return null;
        }

        public async Task<(LmsUserDTO result, string error)> GetEnrollmentAsync(
            Dictionary<string, object> licenseSettings,
            string enrollmentId,
            object extraData = null)
        {
            if (_buzzClient.DomainId == null)
            {
                var loginResponse = await InitializeClient(licenseSettings);
                if (loginResponse == null || _buzzClient.DomainId == null)
                {
                    return (null, "Unable to login to Buzz API");
                }
            }

            var enrollmentResult = await _buzzClient.GetAsync(Commands.Enrollments.GetOne,
                    string.Format(Parameters.Enrollments.GetOne, enrollmentId).ToDictionary());

            if (enrollmentResult == null)
            {
                return (null, "Unable to retrive enrollment from API");
            }

            var enrollment = enrollmentResult.XPathSelectElement("/enrollment");
            if (enrollment != null)
            {
                var userId = enrollment.XPathEvaluate("string(@userid)").ToString();
                var courseId = int.Parse(enrollment.XPathEvaluate("string(@courseid)").ToString());
                var role = enrollment.XPathEvaluate("string(@privileges)").ToString();
                var status = enrollment.XPathEvaluate("string(@status)").ToString();
                var user = enrollment.XPathSelectElement("user");
                var email = user.XPathEvaluate("string(@email)").ToString();
                var userName = user.XPathEvaluate("string(@username)").ToString();

                return (new LmsUserDTO
                {
                    LmsRole = role,
                    Email = email,
                    Login = userName,
                    Id = userId,
                    Name = userName,
                }, null);
            }

            var error = "Enrollment not found";

            return (null, error);
        }

        #endregion

        #region Methods

        internal async Task<XElement> InitializeClient(Dictionary<string, object> licenseSettings)
        {
            string lmsDomain = licenseSettings[LmsLicenseSettingNames.LmsDomain].ToString();
            string userLogin = licenseSettings[LmsLicenseSettingNames.BuzzAdminUsername].ToString();
            string password = licenseSettings[LmsLicenseSettingNames.BuzzAdminPassword].ToString();
            try
            {
                var uri = new Uri(
                    lmsDomain.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? lmsDomain
                        : "http://" + lmsDomain
                );

                string userPrefix = uri.Host.ToLower()
                    .Replace(".agilixbuzz.com", string.Empty)
                    .Replace("www.", string.Empty);

                XElement result = await _buzzClient.LoginAsync(userPrefix, userLogin, password); //sets DomainId internally
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("EdugameCloud.Lti.AgilixBuzz.DlapAPI.LoginAndCreateASession", ex);

                return null;
            }
        }

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

        #endregion
    }
}
