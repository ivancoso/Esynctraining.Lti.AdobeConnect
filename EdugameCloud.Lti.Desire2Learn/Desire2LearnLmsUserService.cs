using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.OAuth.Desire2Learn;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
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

        public override LmsUserDTO GetUser(LmsCompany lmsCompany, string lmsUserId, int courseId, out string error, object extraData = null)
        {
            
            if (!CanRetrieveUsersFromApiForCompany(lmsCompany))
            {
                var param = extraData as LtiParamDTO;

                var isCurrentUserAndAdmin =
                    param != null
                    && param.lms_user_id == lmsUserId
                    && ((param.roles != null && param.roles.Contains("Administrator"))
                        || (param.ext_d2l_role != null && param.ext_d2l_role.ToLower().Contains("administrator")));

                //if current user is admin but not allowed to call api - process 'error' parameter in call stack
                if (isCurrentUserAndAdmin)
                {
                    error = $"[GetD2LUsers] AdminUser is not set for LmsCompany with id={lmsCompany.Id}";
                    return null;
                }
            }

            return GetUsersOldStyle(lmsCompany, lmsUserId, courseId, out error, extraData)
                .FirstOrDefault(u => u.id == lmsUserId);
        }

        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error, extraData);
            return OperationResultWithData<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany,
            string lmsUserId, int courseId, out string error, object param = null)
        {
            error = null; // todo: set when something is wrong
            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser == null)
            {
                this.logger.ErrorFormat("[GetD2LUsers] AdminUser is not set for LmsCompany with id={0}", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            if (string.IsNullOrEmpty(lmsUser.Token))
            {
                this.logger.WarnFormat("[GetD2LUsers]: Token does not exist for LmsUser with id={0}. LmsCompany ID: {1}.", lmsUser.Id, lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            string[] tokens = lmsUser.Token.Split(' ');

            // get course users list
            var classlistEnrollments = d2lApiService.GetApiObjects<List<ClasslistUser>>(
                tokens[0],
                tokens[1],
                lmsCompany.LmsDomain,
                string.Format(
                    d2lApiService.EnrollmentsClasslistUrlFormat,
                    (string)this.settings.BrightspaceApiVersion,
                    courseId),
                lmsCompany);

            // mapping to LmsUserDTO
            var result = new List<LmsUserDTO>();
            if (classlistEnrollments != null)
            {
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
                            (string) this.settings.BrightspaceApiVersion,
                            courseId)
                        + (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty),
                        lmsCompany);
                    if (enrollments == null || enrollments.Items == null)
                    {
                        error = "Incorrect API call or returned data. Please contact site administrator";
                        this.logger.Error("[D2L Enrollments]: Object returned from API has null value");
                        return new List<LmsUserDTO>();
                    }

                    enrollmentsList.AddRange(enrollments.Items);
                } while (enrollments.PagingInfo.HasMoreItems && !string.IsNullOrEmpty(enrollments.PagingInfo.Bookmark));


                // current is not enrolled to this course (user is admin) -> add him to user list
                if (AllowAdminAdditionToCourse && classlistEnrollments.All(x => x.Identifier != lmsUserId))
                {
                    var currentLmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(lmsUserId, lmsCompany.Id).Value;

                    if ((currentLmsUser != null) && !string.IsNullOrEmpty(currentLmsUser.Token))
                    {
                        string[] currentUserTokens = currentLmsUser.Token.Split(' ');

                        var currentUserInfo = d2lApiService.GetApiObjects<WhoAmIUser>(
                            currentUserTokens[0],
                            currentUserTokens[1],
                            lmsCompany.LmsDomain,
                            string.Format(d2lApiService.WhoAmIUrlFormat, (string)this.settings.BrightspaceApiVersion),
                            lmsCompany);

                        if (currentUserInfo != null)
                        {
                            //
                            var userInfo = d2lApiService.GetApiObjects<UserData>(tokens[0], tokens[1],
                                lmsCompany.LmsDomain,
                                string.Format(d2lApiService.GetUserUrlFormat, (string)this.settings.BrightspaceApiVersion,
                                    currentUserInfo.Identifier),
                                lmsCompany);

                            classlistEnrollments.Add(
                                new ClasslistUser
                                {
                                    Identifier = currentUserInfo.Identifier,
                                    Username = currentUserInfo.UniqueName,
                                    DisplayName = currentUserInfo.FirstName + " " + currentUserInfo.LastName,
                                    Email = userInfo.ExternalEmail, // TODO: is it OK??
                                });
                        }
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
                        name = enrollment.GetValidFullName(),
                        primary_email = enrollment.Email ?? userInfo?.User.EmailAddress,
                        lms_role = userInfo != null ? userInfo.Role.Name : "Unknown",
                    };
                    result.Add(user);
                }
            }

            return result;
        }

        protected virtual bool AllowAdminAdditionToCourse
        {
            get { return true; }
        }

    }

    public class Desire2LearnLmsUserServiceSync : Desire2LearnLmsUserService
    {
        public Desire2LearnLmsUserServiceSync(ILogger logger, LmsUserModel lmsUserModel, IDesire2LearnApiService d2lApiService, ApplicationSettingsProvider settings)
            : base(logger, lmsUserModel, d2lApiService, settings)
        {
        }


        protected override bool AllowAdminAdditionToCourse
        {
            get { return false; }
        }

    }

}