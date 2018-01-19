using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI _canvasApi;


        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger) : base(logger)
        {
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }


        public override Task<(LmsUserDTO user, string error)> GetUser(ILmsLicense lmsCompany,
            string lmsUserId, int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            if (lmsCompany.AdminUser == null)
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}.", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var token = lmsCompany.AdminUser.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var user = _canvasApi.GetCourseUser(
                lmsUserId,
                lmsCompany,
                token,
                courseId);

            return Task.FromResult<(LmsUserDTO user, string error)>((user, null));
        }

        public override Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            if (lmsCompany.AdminUser == null)
            {
                var message = string.Format("There is no admin user set for LmsCompanyId={0}.CourseId={1}", lmsCompany.Id, courseId);
                Logger.Error(message);
                return Task.FromResult(OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin));
            }

            if (string.IsNullOrWhiteSpace(lmsCompany.AdminUser.Token))
            {
                Logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                return Task.FromResult(OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin));
            }

            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);

            return Task.FromResult(users.ToSuccessResult());
        }

        public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);
            return Task.FromResult<(List<LmsUserDTO> users, string error)>((GroupUsers(users), null));
        }


        private List<LmsUserDTO> FetchUsers(ILmsLicense lmsCompany, int courseId)
        {
            string token = lmsCompany.AdminUser.Token;
            List<LmsUserDTO> users = _canvasApi.GetUsersForCourse(
                lmsCompany.LmsDomain,
                token,
                courseId);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.PrimaryEmail)))
            {
                // TODO: details about ampty email users
                Logger.ErrorFormat("[Canvas GetUsers] API did not return emails. CourseID={0}. LMSCompanyID:{1}.", courseId, lmsCompany.Id);
            }

            return users;
        }

    }

}