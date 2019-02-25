using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.Dto.OAuth.Desire2Learn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Desire2Learn
{
    public class Desire2LearnLmsUserService : LmsUserServiceBase
    {
        private readonly IDesire2LearnApiService d2lApiService;
        private readonly dynamic _settings;


        public Desire2LearnLmsUserService(ILogger logger, IDesire2LearnApiService d2lApiService,
            ApplicationSettingsProvider settings
            ) : base(logger)
        {
            this.d2lApiService = d2lApiService;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings)); ;
        }

        public override async Task<OperationResultWithData<LmsUserDTO>> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, ILtiParam extraData = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            var param = extraData;
            //todo: to license/session settings
            var isCurrentUserAndAdmin =
                param != null
                && param.lms_user_id == lmsUserId
                && ((param.roles != null && param.roles.Contains("Administrator"))
                    || (param.ext_d2l_role != null && param.ext_d2l_role.ToLower().Contains("administrator")));

            //if current user is admin but not allowed to call api - process 'error' parameter in call stack
            if (isCurrentUserAndAdmin)
            {
                var error = $"[GetD2LUsers] AdminUser is not set for LmsCompany with key={licenseSettings[LmsLicenseSettingNames.LicenseKey]}";
                return OperationResultWithData<LmsUserDTO>.Error(error);
            }

            var result = await GetUsers(licenseSettings, courseId, extraData);
            return result.IsSuccess
                ? result.Data.FirstOrDefault(u => u.Id == lmsUserId)?.ToSuccessResult()
                : OperationResultWithData<LmsUserDTO>.Error(result.Message);
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiParam param = null)
        {
            if (licenseSettings == null)
                throw new ArgumentNullException(nameof(licenseSettings));

            if (string.IsNullOrEmpty((string)licenseSettings[LmsUserSettingNames.Token]))
            {
                Logger.WarnFormat("[GetD2LUsers]: Admin Token does not exist for LmsCompany key: {1}.", licenseSettings[LmsLicenseSettingNames.LicenseKey]);
                throw new WarningMessageException("There is no admin user set for the LMS license. Please check integration guides.");
            }

            string[] tokens = ((string)licenseSettings[LmsUserSettingNames.Token]).Split(' ');

            // get course users list
            var classlistEnrollments = d2lApiService.GetApiObjects<List<ClasslistUser>>(
                tokens[0],
                tokens[1],
                (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                string.Format(
                    d2lApiService.EnrollmentsClasslistUrlFormat,
                    (string)_settings.BrightspaceApiVersion,
                    courseId),
                licenseSettings);

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
                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain],
                        string.Format(
                            d2lApiService.EnrollmentsUrlFormat,
                            (string)_settings.BrightspaceApiVersion,
                            courseId)
                        + (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty),
                        licenseSettings);
                    if (enrollments == null || enrollments.Items == null)
                    {
                        var error = "Incorrect API call or returned data. Please contact site administrator";
                        Logger.Error("[D2L Enrollments]: Object returned from API has null value");
                        return OperationResultWithData<List<LmsUserDTO>>.Error(error);
                    }

                    enrollmentsList.AddRange(enrollments.Items);
                } while (enrollments.PagingInfo.HasMoreItems && !string.IsNullOrEmpty(enrollments.PagingInfo.Bookmark));


                string currentLmsUserId = param?.lms_user_id;

                // current user is not enrolled to this course (user is admin) -> add him to user list
                if (((bool)licenseSettings[LmsLicenseSettingNames.BrightSpaceAllowAdminAdditionToCourse] && AllowAdminAdditionToCourse) && classlistEnrollments.All(x => x.Identifier != currentLmsUserId))
                {
                    if (param != null)
                    {
                        classlistEnrollments.Add(
                            new ClasslistUser
                            {
                                Identifier = param.lms_user_id,
                                Username = param.lms_user_login,
                                DisplayName = param.PersonNameGiven + " " + param.PersonNameFamily,
                                Email = param.lis_person_contact_email_primary, // TODO: is it OK??
                                });
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
                        Email = enrollment.Email ?? userInfo?.User.EmailAddress,
                        LmsRole = userInfo != null ? userInfo.Role.Name : "Unknown",
                    };
                    result.Add(user);
                }
            }

            return result.ToSuccessResult();
        }

        protected virtual bool AllowAdminAdditionToCourse
        {
            get { return true; }
        }

    }

    public class Desire2LearnLmsUserServiceSync : Desire2LearnLmsUserService
    {
        public Desire2LearnLmsUserServiceSync(ILogger logger, IDesire2LearnApiService d2lApiService, ApplicationSettingsProvider settings)
            : base(logger, d2lApiService, settings)
        {
        }


        protected override bool AllowAdminAdditionToCourse
        {
            get { return false; }
        }

    }
}
