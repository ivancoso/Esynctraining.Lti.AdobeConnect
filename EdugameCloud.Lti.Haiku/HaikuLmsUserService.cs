using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Haiku;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuLmsUserService : LmsUserServiceBase
    {
        private readonly IHaikuRestApiClient _restApiClient;


        public HaikuLmsUserService(ILogger logger, IHaikuRestApiClient restApiClient) : base(logger)
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

            var result = await _restApiClient.GetUsersForCourseAsync(lmsCompany, courseId);

            return result;
        }
    }

}