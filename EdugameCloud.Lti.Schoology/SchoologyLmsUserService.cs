using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            string error;
            var users = GetUsersOldStyle(lmsCompany, courseId, out error, extraData);
            return users.ToSuccessResult();
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, out string error, LtiParamDTO param = null)
        {
            error = null;
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            //var course = _restApiClient.GetRestCall<RootObject>(lmsCompany.AdminUser.Username,
            //    lmsCompany.AdminUser.Password,
            //    $"courses/{courseId}/sections").Result;

            string clientId = lmsCompany.GetSetting<string>(LmsCompanySettingNames.SchoologyConsumerKey);
            string clientSecret = lmsCompany.GetSetting<string>(LmsCompanySettingNames.SchoologyConsumerSecret);

            var section = _restApiClient.GetRestCall<Section>(clientId,
                clientSecret,
                $"sections/{courseId}").Result;

            var enrollments = new List<Enrollment>();
            object localLockObject = new object();

            //Parallel.ForEach<Section, List<Enrollment>>(
            //      course.section,
            //      () => new List<Enrollment>(),
            //      (section, state, localList) =>
            //      {
            //          var enrollmentCallResult = new SchoologyRestApiClient().GetRestCall<RootObject2>(clientId,
            //              clientSecret,
            //              $"sections/{section.id}/enrollments").Result;
            //          foreach (var enrollment in enrollmentCallResult.enrollment)
            //              localList.Add(enrollment);
            //          return localList;
            //      },
            //      (finalResult) =>
            //      {
            //          lock (localLockObject)
            //              enrollments.AddRange(finalResult);
            //      }
            //);

            var enrollmentCallResult = new SchoologyRestApiClient().GetRestCall<RootObject2>(clientId,
                clientSecret,
                $"sections/{section.id}/enrollments").Result;
            enrollments = enrollmentCallResult.enrollment;
            enrollments = enrollments.Distinct().ToList();

            var enrolledUsers = new List<User>();
            Parallel.ForEach<Enrollment, List<User>>(
                  enrollments.GroupBy(u => u.uid).Select(g => g.First()),
                  () => new List<User>(),
                  (enrollment, state, localList) =>
                  {
                      var usr = new SchoologyRestApiClient().GetRestCall<User>(clientId,
                          clientSecret,
                          $"users/{enrollment.uid}").Result;
                      usr.admin = enrollment.admin;
                      localList.Add(usr);
                      return localList;
                  },
                  (finalResult) =>
                  {
                      lock (localLockObject)
                          enrolledUsers.AddRange(finalResult);
                  }
            );

            return enrolledUsers
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
            
        }
    }

}