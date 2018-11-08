using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("calendar")]
    public class CalendarController : BaseApiController
    {
        private readonly LmsFactory _lmsFactory;


        public CalendarController(
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache,
            LmsFactory lmsFactory
        )
            : base(acAccountService, settings, logger, cache)
        {
            _lmsFactory = lmsFactory ?? throw new ArgumentNullException(nameof(lmsFactory));
        }


        [Route("createBatch")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<IEnumerable<MeetingSessionDTO>>> CreateBatch([FromBody]CreateMeetingSessionsBatchDto dto)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService(LmsCompany, param);
                var result = await meetingSessionService.CreateBatchAsync(dto, param);
                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateBatchEvents", ex);

                return OperationResultWithData<IEnumerable<MeetingSessionDTO>>.Error(errorMessage);
            }
        }

        [Route("getevents")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<IEnumerable<MeetingSessionDTO>>> GetEvents([FromBody]MeetingRequestDto request)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService(LmsCompany, param);
                var result = await meetingSessionService.GetSessions(request.MeetingId);
                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessions", ex);
                return OperationResultWithData<IEnumerable<MeetingSessionDTO>>.Error(errorMessage);
            }
        }

        [Route("createevent")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<MeetingSessionDTO>> CreateEvent([FromBody]CreateEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService(LmsCompany, param);
                var eve = await meetingSessionService.CreateSessionAsync(model.MeetingId, param);
                return eve.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateSession", ex);
                return OperationResultWithData<MeetingSessionDTO>.Error(errorMessage);
            }
        }

        [Route("saveevent")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<MeetingSessionDTO>> SaveEvent([FromBody]SaveMeetingEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService(LmsCompany, param);
                var eve = await meetingSessionService.SaveSessionAsync(model.MeetingId, model, param);
                return eve.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSession", ex);
                return OperationResultWithData<MeetingSessionDTO>.Error(errorMessage);
            }
        }

        [Route("deleteevent")]
        [HttpPost]
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResult> DeleteEvent([FromBody]DeleteMeetingEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService(LmsCompany, param);
                await meetingSessionService.DeleteSessionAsync(model.MeetingId, model.Id.GetValueOrDefault(), param);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSession", ex);
                return OperationResult.Error(errorMessage);
            }
        }

    }

}
