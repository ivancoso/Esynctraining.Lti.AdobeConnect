using System;
using System.Collections.Generic;
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


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
//            if (lmsUser == null)
//                throw new ArgumentNullException(nameof(lmsUser));

            string error;
            var users = GetUsersOldStyle(lmsCompany, courseId, out error);
            return error != null
                ? OperationResultWithData<List<LmsUserDTO>>.Error(error)
                : users.ToSuccessResult();
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, out string error, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            List<LmsUserDTO> users = _moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            return GroupUsers(users);
        }

    }

}