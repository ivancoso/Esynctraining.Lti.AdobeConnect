using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.Moodle
{
    public class MoodleLmsUserService : LmsUserServiceBase
    {
        private readonly IMoodleApi moodleApi;

        public MoodleLmsUserService(ILogger logger, IMoodleApi moodleApi) : base(logger)
        {
            this.moodleApi = moodleApi;
        }

        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error);
            return error != null
                ? OperationResultWithData<List<LmsUserDTO>>.Error(error)
                : OperationResultWithData<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string lmsUserId, int courseId, out string error, object param = null)
        {
            List<LmsUserDTO> users = this.moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            return GroupUsers(users);
        }
    }
}