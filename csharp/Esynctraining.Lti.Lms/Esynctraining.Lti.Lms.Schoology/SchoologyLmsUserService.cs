using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API.Schoology;

namespace Esynctraining.Lti.Lms.Schoology
{
    public class SchoologyLmsUserService : LmsUserServiceBase
    {
        private readonly ISchoologyRestApiClient _restApiClient;

        public SchoologyLmsUserService(ILogger logger, ISchoologyRestApiClient restApiClient) : base(logger)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
        }

        public override async Task<OperationResultWithData<LmsUserDTO>> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, ILtiParam extraData = null)
        {
            if (lmsUserId.Contains("::"))
            {
                lmsUserId = lmsUserId.Substring(0, lmsUserId.IndexOf("::"));
            }

            string clientId = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerKey].ToString();
            string clientSecret = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerSecret].ToString();

            var usr = await _restApiClient.GetRestCall<User>(clientId,
                clientSecret,
                $"users/{lmsUserId}");

            var lmsUser = new LmsUserDTO
            {
                Id = usr.uid,
                Login = string.IsNullOrWhiteSpace(usr.username) ? usr.primary_email : usr.username,
                // TODO: middle name
                Name = usr.name_first + " " + usr.name_last,
                Email = usr.primary_email,
                LmsRole = usr.admin == 1 ? "Teacher" : "Student",
            };

            return lmsUser.ToSuccessResult();
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiParam extraData = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            string clientId = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerKey].ToString();
            string clientSecret = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerSecret].ToString();

            var section = await _restApiClient.GetRestCall<Section>(clientId,
                clientSecret,
                $"sections/{courseId}");

            List<Enrollment> enrollments = new List<Enrollment>();
            EnrollmentsResult enrollmentCallResult;
            int startPage = 0;
            int pageSize = 100;
            do
            {
                enrollmentCallResult = await _restApiClient.GetRestCall<EnrollmentsResult>(clientId,
                    clientSecret,
                    $"sections/{section.id}/enrollments?start={startPage}&limit={pageSize}");

                enrollments.AddRange(enrollmentCallResult.enrollment);

                startPage += pageSize;

            } while (enrollments.Count < Int32.Parse(enrollmentCallResult.total));
            
            startPage = 0;
            UsersResult usersCallResult;
            var users = new List<User>();
            do
            {
                usersCallResult = await _restApiClient.GetRestCall<UsersResult>(clientId,
                    clientSecret,
                    $"users?start={startPage}&limit={pageSize}");

                users.AddRange(usersCallResult.user);

                startPage += pageSize;

            } while (users.Count < Int32.Parse(usersCallResult.total));

            //https://developers.schoology.com/api-documentation/rest-api-v1/enrollment
            //status 5: Archived (Course specific status members can be placed in before being fully unenrolled)

            var enrolledUsers =
                enrollments.Distinct().Where(e => e.status != "5").GroupBy(u => u.uid).Select(g =>
                {
                    var enr = g.First();
                    var user = users.FirstOrDefault(U => U.uid == enr.uid);
                    if (user == null)
                        return null;
                    return new LmsUserDTO
                    {
                        Id = user.uid,
                        Login = string.IsNullOrWhiteSpace(user.username) ? user.primary_email : user.username,
                        // TODO: middle name
                        Name = user.name_first + " " + user.name_last,
                        Email = user.primary_email,
                        LmsRole = enr.admin == 1 ? "Teacher" : "Student"
                    };
                }).Where(x => x != null).ToList();

            return enrolledUsers.ToSuccessResult();
        }
    }
}
