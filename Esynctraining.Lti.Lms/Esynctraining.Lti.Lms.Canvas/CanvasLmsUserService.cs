using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
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
            string lmsUserId, string courseId, ILtiUserListParam extraData = null)
        {
            var user = await _canvasApi.GetCourseUser(lmsUserId, licenseSettings, courseId);
            return user != null ? user.ToSuccessResult() : OperationResultWithData<LmsUserDTO>.Error("User not found in course");
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiUserListParam extraData = null)
        {
            List<LmsUserDTO> users = await FetchUsers(licenseSettings, courseId);
            return users.ToSuccessResult();
        }

        private async Task<List<LmsUserDTO>> FetchUsers(Dictionary<string, object> licenseSettings, string courseId)
        {
            var lmsDomain = (string) licenseSettings[LmsLicenseSettingNames.LmsDomain];
            List<LmsUserDTO> users = await _canvasApi.GetUsersForCourse(lmsDomain, courseId, licenseSettings);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.Email)))
            {
                // TODO: details about ampty email users
                Logger.Error($"[Canvas GetUsers] API did not return emails. CourseID={courseId}. License: '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
            }

            return users;
        }
    }
}