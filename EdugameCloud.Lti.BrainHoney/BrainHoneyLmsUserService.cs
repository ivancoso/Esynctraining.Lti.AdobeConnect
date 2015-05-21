using System.Collections.Generic;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.BrainHoney;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.BrainHoney
{
    public class BrainHoneyLmsUserService : LmsUserServiceBase
    {
        private readonly LmsUserModel lmsUserModel;
        private readonly dynamic settings;
        private readonly IBrainHoneyApi dlapApi;

        public BrainHoneyLmsUserService(ILogger logger, LmsUserModel lmsUserModel, IBrainHoneyApi dlapApi,
            ApplicationSettingsProvider settings
            ) : base(logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.dlapApi = dlapApi;
            this.settings = settings;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, meeting, lmsUser.UserId, courseId, out error, forceUpdate, extraData);
            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, 
            string lmsUserId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            Session session = param == null ? null : (Session)param;
            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(
                lmsCompany,
                courseId,
                out error,
                param);
            return GroupUsers(users);
        }
    }
}