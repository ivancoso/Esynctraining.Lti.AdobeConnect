using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Haiku;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Haiku
{
    public class HaikuLmsUserService : LmsUserServiceBase
    {
        private readonly IHaikuRestApiClient _restApiClient;

        public HaikuLmsUserService(ILogger logger, IHaikuRestApiClient restApiClient) : base(logger)
        {
            _restApiClient = restApiClient ?? throw new ArgumentNullException(nameof(restApiClient));
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiParam extraData = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            var result = await _restApiClient.GetUsersForCourseAsync(licenseSettings, courseId);

            return string.IsNullOrEmpty(result.error)
                ? result.users.ToSuccessResult()
                : OperationResultWithData<List<LmsUserDTO>>.Error(result.error);
        }

    }

}