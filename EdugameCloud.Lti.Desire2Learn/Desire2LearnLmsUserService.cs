using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.OAuth.Desire2Learn;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Desire2Learn
{
    public class Desire2LearnLmsUserService : LmsUserServiceBase
    {
        private readonly LmsUserModel lmsUserModel;
        private readonly dynamic settings;
        private readonly IDesire2LearnApiService d2lApiService;

        public Desire2LearnLmsUserService(ILogger logger, LmsUserModel lmsUserModel, IDesire2LearnApiService d2lApiService,
            ApplicationSettingsProvider settings
            ) : base(logger)
        {
            this.lmsUserModel = lmsUserModel;
            this.d2lApiService = d2lApiService;
            this.settings = settings;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, meeting, lmsUser.UserId, courseId, out error, forceUpdate, extraData);
            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, 
            string lmsUserId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            error = null; // todo: set when something is wrong
            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser == null)
            {
                this.logger.WarnFormat("[GetD2LUsers] AdminUser is not set for LmsCompany with id={0}", lmsCompany.Id);
                lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    return new List<LmsUserDTO>();
                }
            }

            if (string.IsNullOrEmpty(lmsUser.Token))
            {
                this.logger.WarnFormat("[GetD2LUsers]: Token does not exist for LmsUser with id={0}", lmsUser.Id);
                return new List<LmsUserDTO>();
            }

            string[] tokens = lmsUser.Token.Split(' ');

            // get course users list
            var classlistEnrollments = d2lApiService.GetApiObjects<List<ClasslistUser>>(
                tokens[0],
                tokens[1],
                lmsCompany.LmsDomain,
                string.Format(
                    d2lApiService.EnrollmentsClasslistUrlFormat,
                    (string)this.settings.D2LApiVersion,
                    courseId));

            // get enrollments - this information contains user roles
            var enrollmentsList = new List<OrgUnitUser>();
            PagedResultSet<OrgUnitUser> enrollments = null;
            do
            {
                enrollments = d2lApiService.GetApiObjects<PagedResultSet<OrgUnitUser>>(
                    tokens[0],
                    tokens[1],
                    lmsCompany.LmsDomain,
                    string.Format(
                        d2lApiService.EnrollmentsUrlFormat,
                        (string)this.settings.D2LApiVersion,
                        courseId)
                    + (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty));
                if (enrollments == null || enrollments.Items == null)
                {
                    error = "Incorrect API call or returned data. Please contact site administrator";
                    this.logger.Error("[D2L Enrollments]: Object returned from API has null value");
                    return new List<LmsUserDTO>();
                }

                enrollmentsList.AddRange(enrollments.Items);
            }
            while (enrollments.PagingInfo.HasMoreItems);

            // mapping to LmsUserDTO
            var result = new List<LmsUserDTO>();
            if (classlistEnrollments != null)
            {
                // current user is admin and not enrolled to this course -> add him to user list
                if (classlistEnrollments.All(x => x.Identifier != lmsUserId))
                {
                    var currentUserInfo = d2lApiService.GetApiObjects<WhoAmIUser>(
                        tokens[0],
                        tokens[1],
                        lmsCompany.LmsDomain,
                        string.Format(d2lApiService.WhoAmIUrlFormat, (string)this.settings.D2LApiVersion));
                    if (currentUserInfo != null)
                    {
                        classlistEnrollments.Add(
                            new ClasslistUser
                            {
                                Identifier = currentUserInfo.Identifier,
                                Username = currentUserInfo.UniqueName,
                                DisplayName = currentUserInfo.FirstName + " " + currentUserInfo.LastName
                            });
                    }
                }

                foreach (ClasslistUser enrollment in classlistEnrollments)
                {
                    OrgUnitUser userInfo =
                        enrollmentsList.FirstOrDefault(e => e.User.Identifier == enrollment.Identifier);
                    var user = new LmsUserDTO
                    {
                        id = enrollment.Identifier,
                        login_id = enrollment.Username,
                        name = enrollment.DisplayName,
                        primary_email = enrollment.Email,
                        lms_role = userInfo != null ? userInfo.Role.Name : "Unknown"
                    };
                    result.Add(user);
                }
            }

            return result;
        }
    }
}