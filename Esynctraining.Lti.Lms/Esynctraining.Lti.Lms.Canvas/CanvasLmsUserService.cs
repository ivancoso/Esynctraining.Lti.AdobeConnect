using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;


        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger) : base(logger)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }


        public override async Task<OperationResultWithData<LmsUserDTO>> GetUser(Dictionary<string, object> licenseSettings,
            string lmsUserId, string courseId, LtiParamDTO extraData = null)
        {
            if (!licenseSettings.ContainsKey(LmsUserSettingNames.Token))
            {
                Logger.Error($"There is no user token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                throw new WarningMessageException(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
            }

            var token = (string)licenseSettings[LmsUserSettingNames.Token];

            if (string.IsNullOrWhiteSpace(token))
            {
                Logger.Error($"Empty token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                throw new WarningMessageException(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
            }

            var user = await _canvasApi.GetCourseUser(
                lmsUserId,
                licenseSettings,
                token,
                courseId);

            return user != null ? user.ToSuccessResult() : OperationResultWithData<LmsUserDTO>.Error("User not found in course");
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO extraData = null)
        {
            //if (lmsCompany == null)
            //    throw new ArgumentNullException(nameof(lmsCompany));

            if (!licenseSettings.ContainsKey(LmsUserSettingNames.Token))
            {
                Logger.Error($"There is no user token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'. CourseId={courseId}");
                return OperationResultWithData<List<LmsUserDTO>>.Error(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
            }

            if (string.IsNullOrWhiteSpace((string)licenseSettings[LmsUserSettingNames.Token]))
            {
                Logger.Error($"Empty token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                return OperationResultWithData<List<LmsUserDTO>>.Error(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
            }

            List<LmsUserDTO> users = await FetchUsers(licenseSettings, courseId);

            return users.ToSuccessResult();
        }

        public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(Dictionary<string, object> licenseSettings, string courseId, LtiParamDTO param = null)
        {
            throw new NotImplementedException();
        }

        private async Task<List<LmsUserDTO>> FetchUsers(Dictionary<string, object> licenseSettings,
            string courseId)
        {
            string token = (string)licenseSettings[LmsUserSettingNames.Token];
            List<LmsUserDTO> users = await _canvasApi.GetUsersForCourse(
                (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                token,
                courseId);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.PrimaryEmail)))
            {
                // TODO: details about ampty email users
                Logger.Error($"[Canvas GetUsers] API did not return emails. CourseID={courseId}. License: '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
            }

            return users;
        }
    }
}