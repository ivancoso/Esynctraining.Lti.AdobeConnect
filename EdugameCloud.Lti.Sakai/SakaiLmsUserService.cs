using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiLmsUserService : LmsUserServiceBase
    {
        private readonly LTI2Api _lti2Api;


        public SakaiLmsUserService(ILogger logger, LTI2Api lti2Api) : base(logger)
        {
            _lti2Api = lti2Api ?? throw new ArgumentNullException(nameof(lti2Api));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings, 
            string courseId, LtiParamDTO param = null)
        {
            if (param == null)
            {
                return OperationResultWithData<List<LmsUserDTO>>.Error("Extra data is not set.");
            }

            var result = await _lti2Api.GetUsersForCourse(
                licenseSettings,
                param.ext_ims_lis_memberships_url ?? param.ext_ims_lti_tool_setting_url,
                param.ext_ims_lis_memberships_id);

            return string.IsNullOrEmpty(result.Item2)
                ? GroupUsers(result.Item1).ToSuccessResult()
                : OperationResultWithData<List<LmsUserDTO>>.Error(result.Item2);
        }

    }

}