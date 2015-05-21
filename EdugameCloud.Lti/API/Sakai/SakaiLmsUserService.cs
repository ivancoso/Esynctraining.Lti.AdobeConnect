using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.API.Common;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.API.Sakai
{
    public class SakaiLmsUserService : LmsUserServiceBase
    {
        private readonly LmsUserModel lmsUserModel;
        private readonly dynamic settings;
        private readonly LTI2Api lti2Api;

        public SakaiLmsUserService(ILogger logger, LmsUserModel lmsUserModel, LTI2Api lti2Api,
            ApplicationSettingsProvider settings) : base(logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.lti2Api = lti2Api;
            this.settings = settings;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting, 
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, meeting, lmsUser.UserId, courseId, out error, forceUpdate);
            return error != null ? OperationResult<List<LmsUserDTO>>.Success(users): OperationResult<List<LmsUserDTO>>.Error(error);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, string lmsUserId, 
            int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            var paramDto = param as LtiParamDTO;
            if (paramDto as LtiParamDTO != null)
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