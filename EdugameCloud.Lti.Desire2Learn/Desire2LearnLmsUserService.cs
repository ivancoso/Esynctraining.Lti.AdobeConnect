using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public override async Task<(LmsUserDTO user, string error)> GetUser(ILmsLicense lmsCompany, string lmsUserId, int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var param = extraData as LtiParamDTO;

            var isCurrentUserAndAdmin =
                param != null
                && param.lms_user_id == lmsUserId
                && ((param.roles != null && param.roles.Contains("Administrator"))
                    || (param.ext_d2l_role != null && param.ext_d2l_role.ToLower().Contains("administrator")));

            //if current user is admin but not allowed to call api - process 'error' parameter in call stack
            if (isCurrentUserAndAdmin)
            {
                var error = $"[GetD2LUsers] AdminUser is not set for LmsCompany with id={lmsCompany.Id}";
                return (null, error);
            }

            var result = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            if (string.IsNullOrWhiteSpace(result.error))
                return (result.users.FirstOrDefault(u => u.Id == lmsUserId), result.error);

            return (null, result.error);
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            LmsUser lmsUser = lmsCompany.AdminUser;
            if (lmsUser == null)
            {
                Logger.ErrorFormat("[GetD2LUsers] AdminUser is not set for LmsCompany with id={0}", lmsCompany.Id);
                throw new WarningMessageException(Resources.Messages.NoLicenseAdmin);
            }

            if (string.IsNullOrEmpty(lmsUser.Token))
            {
                Logger.WarnFormat("[GetD2LUsers]: Token does not exist for LmsUser with id={0}. LmsCompany ID: {1}.", lmsUser.Id, lmsCompany.Id);
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
                            (string)this.settings.BrightspaceApiVersion,
                            courseId)
                        + (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty),
                        lmsCompany);
                    if (enrollments == null || enrollments.Items == null)
                    {
                        var error = "Incorrect API call or returned data. Please contact site administrator";
                        Logger.Error("[D2L Enrollments]: Object returned from API has null value");
                        return (new List<LmsUserDTO>(), error);
                    }

                    enrollmentsList.AddRange(enrollments.Items);
                } while (enrollments.PagingInfo.HasMoreItems && !string.IsNullOrEmpty(enrollments.PagingInfo.Bookmark));


                string currentLmsUserId = param?.lms_user_id;

                // current user is not enrolled to this course (user is admin) -> add him to user list
                if (AllowAdminAdditionToCourse && classlistEnrollments.All(x => x.Identifier != currentLmsUserId))
                {
                    var currentLmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(currentLmsUserId, lmsCompany.Id).Value;

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
                        Id = enrollment.Identifier,
                        Login = enrollment.Username,
                        Name = enrollment.GetValidFullName(),
                        PrimaryEmail = enrollment.Email ?? userInfo?.User.EmailAddress,
                        LmsRole = userInfo != null ? userInfo.Role.Name : "Unknown",
                    };
                    result.Add(user);
                }
            }

            return (result, null);
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