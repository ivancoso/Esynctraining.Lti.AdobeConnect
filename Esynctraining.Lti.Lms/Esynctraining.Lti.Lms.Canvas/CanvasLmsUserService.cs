using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Domain;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;


        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger) : base(logger)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }


        public override async Task<(LmsUserDTO user, string error)> GetUser(Dictionary<string, object> licenseSettings,
            string lmsUserId, string courseId, LtiParamDTO extraData = null)
        {
            //if (lmsCompany == null)
            //    throw new ArgumentNullException(nameof(lmsCompany));

            //if (lmsCompany.AdminUser == null)
            if (!licenseSettings.ContainsKey("AdminUser.Token"))
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}.", licenseSettings["lmsCompany.Id"]);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var token = (string)licenseSettings["AdminUser.Token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", licenseSettings["lmsCompany.Id"]);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var user = await _canvasApi.GetCourseUser(
                lmsUserId,
                licenseSettings,
                token,
                courseId);

            return (user, null);
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, LtiParamDTO extraData = null)
        {
            //if (lmsCompany == null)
            //    throw new ArgumentNullException(nameof(lmsCompany));

            //if (lmsCompany.AdminUser == null)
            //if (lmsCompany.AdminUser == null)
            if (!licenseSettings.ContainsKey("AdminUser.Token"))
            {
                var message =
                    $"There is no admin user set for LmsCompanyId={licenseSettings["lmsCompany.Id"]}.CourseId={courseId}";
                Logger.Error(message);
                return OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin);
            }

            if (string.IsNullOrWhiteSpace((string)licenseSettings["AdminUser.Token"]))
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", licenseSettings["lmsCompany.Id"]);
                return OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin);
            }

            List<LmsUserDTO> users = await FetchUsers(licenseSettings, courseId);

            return users.ToSuccessResult();
        }

        //public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, LtiParamDTO param = null)
        //{
        //    if (lmsCompany == null)
        //        throw new ArgumentNullException(nameof(lmsCompany));

        //    List<LmsUserDTO> users = await FetchUsers(lmsCompany, courseId);
        //    return (GroupUsers(users), null);
        //}


        private async Task<List<LmsUserDTO>> FetchUsers(Dictionary<string, object> licenseSettings,
            string courseId)
        {
            string token = (string)licenseSettings["AdminUser.Token"];
            List<LmsUserDTO> users = await _canvasApi.GetUsersForCourse(
                (string)licenseSettings["LmsDomain"],
                token,
                courseId);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.PrimaryEmail)))
            {
                // TODO: details about ampty email users
                Logger.ErrorFormat("[Canvas GetUsers] API did not return emails. CourseID={0}. LMSCompanyID:{1}.", courseId, licenseSettings["lmsCompany.Id"]);
            }

            return users;
        }

    }

}