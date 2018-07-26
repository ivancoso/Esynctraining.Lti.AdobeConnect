﻿using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.AgilixBuzz;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    public class AgilixBuzzLmsUserService : LmsUserServiceBase
    {
        private readonly IAgilixBuzzApi _dlapApi;

        public AgilixBuzzLmsUserService(ILogger logger, IAgilixBuzzApi dlapApi) : base(logger)
        {
            _dlapApi = dlapApi ?? throw new ArgumentNullException(nameof(dlapApi));
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO extraData = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            var users = await GetUsersOldStyle(licenseSettings, courseId, extraData);
            return users.users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO param = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            var (users, error) = await _dlapApi.GetUsersForCourseAsync(licenseSettings, courseId, param);

            if (!string.IsNullOrWhiteSpace(error))
                Logger.Error("[AgilixBuzz.dlapApi.GetUsersForCourse] error:" + error);

            return (GroupUsers(users), error);
        }
    }
}
