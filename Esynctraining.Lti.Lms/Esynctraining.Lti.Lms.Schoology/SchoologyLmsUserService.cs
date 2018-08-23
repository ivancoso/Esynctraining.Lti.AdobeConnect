using EdugameCloud.Lti.API.Schoology;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Schoology
{
    public class SchoologyLmsUserService : LmsUserServiceBase
    {
        private readonly ISchoologyRestApiClient _restApiClient;


        public SchoologyLmsUserService(ILogger logger, ISchoologyRestApiClient restApiClient) : base(logger)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO extraData = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            var users = await GetUsersOldStyle(licenseSettings, courseId, extraData);
            return users.users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO param = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            //var course = _restApiClient.GetRestCall<RootObject>(lmsCompany.AdminUser.Username,
            //    lmsCompany.AdminUser.Password,
            //    $"courses/{courseId}/sections").Result;

            string clientId = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerKey].ToString();
            string clientSecret = licenseSettings[LmsLicenseSettingNames.SchoologyConsumerSecret].ToString();

            var section = await _restApiClient.GetRestCall<Section>(clientId,
                clientSecret,
                $"sections/{courseId}");

            List<Enrollment> enrollments = new List<Enrollment>();
            RootObject2 enrollmentCallResult;
            int startPage = 0;
            int pageSize = 20;
            do
            {
                enrollmentCallResult = await _restApiClient.GetRestCall<RootObject2>(clientId,
                    clientSecret,
                    $"sections/{section.id}/enrollments?start={startPage}&limit={pageSize}");

                enrollments.AddRange(enrollmentCallResult.enrollment);

                startPage += pageSize;

            } while (enrollments.Count < Int32.Parse(enrollmentCallResult.total));

            //https://developers.schoology.com/api-documentation/rest-api-v1/enrollment
            //status 5: Archived (Course specific status members can be placed in before being fully unenrolled)
            enrollments = enrollments.Distinct().Where(e => e.status != "5").ToList();

            var enrolledUserTasks = enrollments.GroupBy(u => u.uid).Select(g => g.First()).Select(enrollment =>
            {
                var usr = new SchoologyRestApiClient().GetRestCall<User>(clientId,
                          clientSecret,
                          $"users/{enrollment.uid}");
                return usr.ContinueWith(x =>
                {
                    var u = x.Result;
                    u.admin = enrollment.admin;
                    return u;
                });
            });

            var enrolledUsers = await Task.WhenAll(enrolledUserTasks);

            var users = enrolledUsers
                .GroupBy(u => u.uid)
                .Select(g => g.First())
                .Select(x => new LmsUserDTO
                {
                    Id = x.uid,
                    Login = string.IsNullOrWhiteSpace(x.username) ? x.primary_email : x.username,
                    // TODO: middle name
                    Name = x.name_first + " " + x.name_last,
                    Email = x.primary_email,
                    LmsRole = x.admin == 1 ? "Teacher" : "Student",
                    PrimaryEmail = x.primary_email,
                })
                .ToList();
            return (users, null);
        }
    }
}
