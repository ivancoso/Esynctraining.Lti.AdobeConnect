using System;
using System.Collections.Generic;
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


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            string error;
            var users = GetUsersOldStyle(lmsCompany, courseId, out error, extraData);
            return error != null
                ? OperationResultWithData<List<LmsUserDTO>>.Error(error)
                : users.ToSuccessResult();
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, out string error, LtiParamDTO param = null)
        {
            error = null;

            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var result = _restApiClient.GetUsersForCourse(lmsCompany, courseId, out error);

            return result;
        }
    }

}