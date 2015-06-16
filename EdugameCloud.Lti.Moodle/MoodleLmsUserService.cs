using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Moodle
{
    public class MoodleLmsUserService : LmsUserServiceBase
    {
        private readonly LmsUserModel lmsUserModel;
        private readonly dynamic settings;
        private readonly IMoodleApi moodleApi;

        public MoodleLmsUserService(ILogger logger, LmsUserModel lmsUserModel, IMoodleApi moodleApi,
            ApplicationSettingsProvider settings
            ) : base(logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.moodleApi = moodleApi;
            this.settings = settings;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, meeting, lmsUser.UserId, courseId, out error, forceUpdate);
            return error != null
                ? OperationResult<List<LmsUserDTO>>.Error(error)
                : OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, string lmsUserId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            List<LmsUserDTO> users = this.moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            return GroupUsers(users);
        }
    }
}