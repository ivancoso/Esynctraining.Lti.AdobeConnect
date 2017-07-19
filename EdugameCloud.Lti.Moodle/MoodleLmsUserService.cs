using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.Core.Constants;
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


        public override bool CanRetrieveUsersFromApiForCompany(ILmsLicense lmsCompany)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            return lmsCompany.AdminUser != null 
                || !string.IsNullOrEmpty(lmsCompany.GetSetting<string>(LmsCompanySettingNames.MoodleCoreServiceToken))
                || !string.IsNullOrEmpty(lmsCompany.GetSetting<string>(LmsCompanySettingNames.SchoologyConsumerKey));
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

            List<LmsUserDTO> users = this.moodleApi.GetUsersForCourse(lmsCompany, courseId, out error);
            return GroupUsers(users);
        }

    }

}