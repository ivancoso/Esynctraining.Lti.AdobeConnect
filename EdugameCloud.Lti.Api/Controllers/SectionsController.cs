using System;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    public class SectionsController : BaseApiController
    {
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly LmsFactory lmsFactory;

        public SectionsController(
            MeetingSetup meetingSetup, UsersSetup usersSetup, LmsFactory lmsFactory,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            this.meetingSetup = meetingSetup;
            this.usersSetup = usersSetup;
            this.lmsFactory = lmsFactory;
        }

        [Route("meeting/sections")]
        [HttpGet]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResult GetMeetingCourseSections()
        {
            if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.UseCourseSections))
            {
                return OperationResult.Error("License doesn't support 'Sections' feature");
            }

            var sectionsService = lmsFactory.GetCourseSectionsService((LmsProviderEnum)LmsCompany.LmsProviderId);
            return sectionsService.GetCourseSections(LmsCompany, CourseId.ToString()).ToSuccessResult();
        }

        [Route("meeting/UpdateMeetingCourseSections")]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual async Task<OperationResult> UpdateMeetingCourseSections([FromBody]UpdateCourseSectionsDto updateCourseSectionsDto)
        {
            if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.UseCourseSections))
            {
                return OperationResult.Error("License doesn't support 'Sections' feature");
            }
            var param = Session.LtiSession.LtiParam;
            var ac = GetAdminProvider();
            try
            {
                meetingSetup.UpdateMeetingCourseSections(LmsCompany, updateCourseSectionsDto);

                var users = await usersSetup.GetUsers(LmsCompany,
                    ac,
                    param.course_id,
                    param,
                    updateCourseSectionsDto.MeetingId);
                return users.Item2 != null
                    ? OperationResult.Error(users.Item2)
                    : users.Item1.ToSuccessResult();


            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeetingCourseSections", ex);
                return OperationResult.Error(errorMessage);
            }
        }
    }
}