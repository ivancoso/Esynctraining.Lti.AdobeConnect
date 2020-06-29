using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Schoology;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Schoology
{
    public class SchoologyLmsUserService : LmsUserServiceBase
    {
        private readonly ISchoologyRestApiClient _restApiClient;


        public SchoologyLmsUserService(ILogger logger, ISchoologyRestApiClient restApiClient) : base(logger)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            //var course = _restApiClient.GetRestCall<RootObject>(lmsCompany.AdminUser.Username,
            //    lmsCompany.AdminUser.Password,
            //    $"courses/{courseId}/sections").Result;

            string clientId = lmsCompany.GetSetting<string>(LmsCompanySettingNames.SchoologyConsumerKey);
            string clientSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.SchoologyConsumerSecret);

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