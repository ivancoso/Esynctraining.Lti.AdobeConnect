using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiLmsUserService : LmsUserServiceBase
    {
        private readonly LTI2Api _lti2Api;


        public SakaiLmsUserService(ILogger logger, LTI2Api lti2Api) : base(logger)
        {
            _lti2Api = lti2Api ?? throw new ArgumentNullException(nameof(lti2Api));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany, 
            int courseId, LtiParamDTO extraData = null)
        {
            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.Item1.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, LtiParamDTO param = null)
        {
            if (param == null)
            {
                return (new List<LmsUserDTO>(), "Extra data is not set.");
            }

            var result = await _lti2Api.GetUsersForCourse(
                lmsCompany,
                param.ext_ims_lis_memberships_url ?? param.ext_ims_lti_tool_setting_url,
                param.ext_ims_lis_memberships_id);

            return (GroupUsers(result.Item1), result.Item2);
        }

    }

}