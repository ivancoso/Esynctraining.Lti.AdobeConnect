using System.Collections.Generic;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Sakai
{
    public class SakaiLmsUserService : LmsUserServiceBase
    {
        private readonly LTI2Api lti2Api;

        public SakaiLmsUserService(ILogger logger, LTI2Api lti2Api) : base(logger)
        {
            this.lti2Api = lti2Api;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, 
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error, forceUpdate, extraData);
            return error != null ? OperationResult<List<LmsUserDTO>>.Success(users): OperationResult<List<LmsUserDTO>>.Error(error);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string lmsUserId, 
            int courseId, out string error, bool forceUpdate = false, object param = null)
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