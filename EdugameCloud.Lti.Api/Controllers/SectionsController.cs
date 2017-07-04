using System;
using System.Text;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
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
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
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
            var api = lmsFactory.GetCourseSectionsService((LmsProviderEnum)LmsCompany.LmsProviderId);
            return api.GetCourseSections(LmsCompany, CourseId.ToString()).ToSuccessResult();
        }

        [Route("meeting/UpdateMeetingCourseSection")]
        [HttpPost]
        [Filters.LmsAuthorizeBase]
        public virtual OperationResult UpdateMeetingCourseSections([FromBody]UpdateCourseSectionsDto updateCourseSectionsDto)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();

                var ac = this.GetAdminProvider();

                var ret = this.meetingSetup.UpdateMeetingCourseSections(LmsCompany, updateCourseSectionsDto);
                if (ret.IsSuccess)
                {
                    string error;
                    return usersSetup.GetUsers(LmsCompany,
                    ac,
                    param.course_id,
                    param,
                    updateCourseSectionsDto.MeetingId,
                    out error).ToSuccessResult();
                }
                return ret;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeetingCourseSection", ex);
                return OperationResult.Error(errorMessage);
            }
        }
    }
}