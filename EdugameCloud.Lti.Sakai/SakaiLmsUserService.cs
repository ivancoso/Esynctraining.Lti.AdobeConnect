using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiLmsUserService : LmsUserServiceBase
    {
        private readonly LTI2Api lti2Api;


        public SakaiLmsUserService(ILogger logger, LTI2Api lti2Api) : base(logger)
        {
            this.lti2Api = lti2Api ?? throw new ArgumentNullException(nameof(lti2Api));
        }


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany, 
            int courseId, LtiParamDTO extraData = null)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, courseId, out error, extraData);
            return error != null ? users.ToSuccessResult() : OperationResultWithData<List<LmsUserDTO>>.Error(error);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany,  int courseId, out string error, LtiParamDTO param = null)
        {
            var paramDto = param as LtiParamDTO;
            if (paramDto != null)
            {
                List<LmsUserDTO> users = this.lti2Api.GetUsersForCourse(
                    lmsCompany,
                    paramDto.ext_ims_lis_memberships_url ?? paramDto.ext_ims_lti_tool_setting_url,
                    paramDto.ext_ims_lis_memberships_id,
                    out error);
                return GroupUsers(users);
            }

            error = "extra data is not set";
            return new List<LmsUserDTO>();
        }

    }

}