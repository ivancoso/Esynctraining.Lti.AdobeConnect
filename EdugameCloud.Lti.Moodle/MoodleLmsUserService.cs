using System.Collections.Generic;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Moodle
{
    public class MoodleLmsUserService : LmsUserServiceBase
    {
        private readonly IMoodleApi moodleApi;

        public MoodleLmsUserService(ILogger logger, IMoodleApi moodleApi) : base(logger)
        {
            this.moodleApi = moodleApi;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error, forceUpdate);
            return error != null
                ? OperationResult<List<LmsUserDTO>>.Error(error)
                : OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string lmsUserId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            List<LmsUserDTO> users = this.moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            return GroupUsers(users);
        }
    }
}