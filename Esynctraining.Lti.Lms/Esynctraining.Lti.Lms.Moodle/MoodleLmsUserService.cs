﻿using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Moodle;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Moodle
{
    public class MoodleLmsUserService : LmsUserServiceBase
    {
        private readonly IMoodleApi _moodleApi;

        public MoodleLmsUserService(ILogger logger, IMoodleApi moodleApi) : base(logger)
        {
            _moodleApi = moodleApi ?? throw new ArgumentNullException(nameof(moodleApi));
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings, string courseId, LtiParamDTO param = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            string error;
            var usersTuple = await _moodleApi.GetUsersForCourse(licenseSettings, courseId);
            List<LmsUserDTO> users = usersTuple.users;
            error = usersTuple.error;

            return !string.IsNullOrEmpty(error)
                ? OperationResultWithData<List<LmsUserDTO>>.Error(error)
                : GroupUsers(users).ToSuccessResult();
        }

    }
}
