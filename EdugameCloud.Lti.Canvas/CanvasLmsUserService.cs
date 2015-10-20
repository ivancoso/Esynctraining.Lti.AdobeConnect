using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Canvas
{
    public class CanvasLmsUserService : LmsUserServiceBase
    {
        private readonly IEGCEnabledCanvasAPI canvasApi;


        public CanvasLmsUserService(IEGCEnabledCanvasAPI canvasApi, ILogger logger) : base(logger)
        {
            this.canvasApi = canvasApi;
        }


        public override LmsUserDTO GetUser(LmsCompany lmsCompany,
            string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");

            if (lmsCompany.AdminUser == null)
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}.", lmsCompany.Id);
                throw new WarningMessageException("There is no admin user set for the LMS license. Please check integration guides.");
            }

            var token = lmsCompany.AdminUser.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                throw new WarningMessageException("There is no admin user set for the LMS license. Please check integration guides.");
            }

            var user = canvasApi.GetCourseUser(
                lmsUserId,
                lmsCompany,
                token,
                courseId);
            
            error = null;
            return user;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException("lmsCompany");
            if (lmsUser == null)
                throw new ArgumentNullException("lmsUser");

            if (lmsCompany.AdminUser == null)
            {
                var message =
                    string.Format("There is no admin user set for LmsCompanyId={0}.CourseId={1}",
                    lmsCompany.Id, courseId);
                logger.Error(message);
                return OperationResult<List<LmsUserDTO>>.Error("There is no admin user set for the LMS license. Please check integration guides.");
            }

            if (string.IsNullOrWhiteSpace(lmsCompany.AdminUser.Token))
            {
                logger.ErrorFormat("There is no admin user set for LmsCompanyId={0}. (AdminUser has EMPTY token).", lmsCompany.Id);
                return OperationResult<List<LmsUserDTO>>.Error("There is no admin user set for the LMS license. Please check integration guides.");
            }

            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);

            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, string userId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            List<LmsUserDTO> users = FetchUsers(lmsCompany, courseId);

            error = null;
            return GroupUsers(users);
        }


        private List<LmsUserDTO> FetchUsers(LmsCompany lmsCompany, int courseId)
        {
            string token = lmsCompany.AdminUser.Token;

            List<LmsUserDTO> users = canvasApi.GetUsersForCourse(
                lmsCompany.LmsDomain,
                token,
                courseId);

            // IF emails are NOT included (for student + lmsCompany.AdminUser == null)
            if (users.Any(x => string.IsNullOrEmpty(x.primary_email)))
            {
                logger.ErrorFormat("[Canvas GetUsers] API did not return emails. CourseID={0}. LMSCompanyID:{1}.", courseId, lmsCompany.Id);
            }

            return users;
        }

    }

}