using System;
using System.Web.Mvc;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly LmsFactory lmsFactory;


        public CalendarController(LmsUserSessionModel userSessionModel, IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings, ILogger logger, LmsFactory lmsFactory, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            this.lmsFactory = lmsFactory ?? throw new ArgumentNullException(nameof(lmsFactory));
        }


        [HttpPost]
        [LmsAuthorize]
        public ActionResult CreateBatch(CreateMeetingSessionsBatchDto dto, LmsUserSession session)
        {
            try
            {
                LtiParamDTO param = session.LtiSession.LtiParam;
                var meetingSessionService = lmsFactory.GetMeetingSessionService((LmsProviderEnum)session.LmsCompany.LmsProviderId);
                var result = meetingSessionService.CreateBatch(dto, param);
                return Json(result.ToSuccessResult());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateBatchEvents", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorizeBase]
        public ActionResult GetEvents(LmsUserSession session, int meetingId)
        {
            try
            {
                var meetingSessionService = lmsFactory.GetMeetingSessionService((LmsProviderEnum)session.LmsCompany.LmsProviderId);
                var result = meetingSessionService.GetSessions(meetingId);
                return Json(result.ToSuccessResult());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessions", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorize]
        public ActionResult CreateEvent(int meetingId, LmsUserSession session)
        {
            try
            {
                LtiParamDTO param = session.LtiSession.LtiParam;
                var meetingSessionService = lmsFactory.GetMeetingSessionService((LmsProviderEnum)session.LmsCompany.LmsProviderId);
                var eve = meetingSessionService.CreateSession(meetingId, param);
                return Json(eve.ToSuccessResult());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateSession", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorize]
        public ActionResult SaveEvent(MeetingSessionDTO ev, int meetingId, LmsUserSession session)
        {
            try
            {
                LtiParamDTO param = session.LtiSession.LtiParam;
                var meetingSessionService = lmsFactory.GetMeetingSessionService((LmsProviderEnum)session.LmsCompany.LmsProviderId);
                var eve = meetingSessionService.SaveSession(meetingId, ev, param);
                return Json(eve.ToSuccessResult());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSession", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        [LmsAuthorize]
        public ActionResult DeleteEvent(int meetingId, int? id, LmsUserSession session)
        {
            try
            {
                LtiParamDTO param = session.LtiSession.LtiParam;
                var meetingSessionService = lmsFactory.GetMeetingSessionService((LmsProviderEnum)session.LmsCompany.LmsProviderId);
                meetingSessionService.DeleteSession(meetingId, id.GetValueOrDefault(), param);
                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSession", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

    }

}
