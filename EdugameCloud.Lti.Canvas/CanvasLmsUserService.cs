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

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI canvasApi;


        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger) : base(logger)
        {
            this.canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
        }


        public override LmsUserDTO GetUser(ILmsLicense lmsCompany,
            string lmsUserId, int courseId, out string error, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            if (lmsCompany.AdminUser == null)
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}.", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var token = lmsCompany.AdminUser.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            var user = canvasApi.GetCourseUser(
                lmsUserId,
                lmsCompany,
                token,
                courseId);
            
            error = null;
            return user;
        }

        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            if (lmsCompany.AdminUser == null)
            {
                var message = string.Format("There is no admin user set for LmsCompanyId={0}.CourseId={1}", lmsCompany.Id, courseId);
                logger.Error(message);
                return OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin);
            }

            if (string.IsNullOrWhiteSpace(lmsCompany.AdminUser.Token))
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                return OperationResultWithData<List<LmsUserDTO>>.Error(Resources.Messages.NoLicenseAdmin);
            }

            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);

            return users.ToSuccessResult();
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, out string error, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);
            error = null;
            return GroupUsers(users);
        }


        private List<LmsUserDTO> FetchUsers(ILmsLicense lmsCompany, int courseId)
        {
            string token = lmsCompany.AdminUser.Token;
            List<LmsUserDTO> users = canvasApi.GetUsersForCourse(
                lmsCompany.LmsDomain,
                token,
                courseId);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.PrimaryEmail)))
            {
                logger.ErrorFormat("[Canvas GetUsers] API did not return emails. CourseID={0}. LMSCompanyID:{1}.", courseId, lmsCompany.Id);
            }

            return users;
        }

    }

}