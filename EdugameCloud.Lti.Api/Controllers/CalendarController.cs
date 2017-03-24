using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("calendar")]
    public partial class CalendarController : BaseApiController
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<MeetingSessionDTO>> CreateBatch([FromBody]CreateMeetingSessionsBatchDto dto)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService((LmsProviderEnum)Session.LmsCompany.LmsProviderId);
                var result = meetingSessionService.CreateBatch(dto, param);
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<IEnumerable<MeetingSessionDTO>> GetEvents(int meetingId)
        {
            try
            {
                var meetingSessionService = _lmsFactory.GetMeetingSessionService((LmsProviderEnum)Session.LmsCompany.LmsProviderId);
                var result = meetingSessionService.GetSessions(meetingId);
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<MeetingSessionDTO> CreateEvent([FromBody]CreateEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService((LmsProviderEnum)Session.LmsCompany.LmsProviderId);
                var eve = meetingSessionService.CreateSession(model.meetingId, param);
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResultWithData<MeetingSessionDTO> SaveEvent([FromBody]SaveMeetingEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService((LmsProviderEnum)Session.LmsCompany.LmsProviderId);
                var eve = meetingSessionService.SaveSession(model.meetingId, model, param);
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResult DeleteEvent([FromBody]DeleteMeetingEventDto model)
        {
            try
            {
                LtiParamDTO param = Session.LtiSession.LtiParam;
                var meetingSessionService = _lmsFactory.GetMeetingSessionService((LmsProviderEnum)Session.LmsCompany.LmsProviderId);
                meetingSessionService.DeleteSession(model.meetingId, model.id.GetValueOrDefault(), param);
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
