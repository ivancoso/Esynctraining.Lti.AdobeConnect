using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Moodle
{
    public class MoodleLmsUserService : LmsUserServiceBase
    {
        private readonly IMoodleApi _moodleApi;


        public MoodleLmsUserService(ILogger logger, IMoodleApi moodleApi) : base(logger)
        {
            _moodleApi = moodleApi ?? throw new ArgumentNullException(nameof(moodleApi));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.Item1.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            string error;
            //List<LmsUserDTO> users = _moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            var usersTuple = await _moodleApi.GetUsersForCourse(lmsCompany, courseId);
            List<LmsUserDTO> users = usersTuple.users;
            error = usersTuple.error;
            return (GroupUsers(users), error);
        }

    }

}