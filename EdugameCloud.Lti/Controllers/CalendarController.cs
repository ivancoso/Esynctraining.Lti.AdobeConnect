using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Lti.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly ICalendarEventService calendarEventService;

        public CalendarController(LmsUserSessionModel userSessionModel, IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings, ILogger logger, ICalendarEventService calendarEventService)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            this.calendarEventService = calendarEventService;
        }

        [HttpPost]
        public ActionResult CreateBatch(CreateCalendarEventsBatchDto dto, string lmsProviderName)
        {
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                LtiParamDTO param = session.LtiSession.LtiParam;
                var result = calendarEventService.CreateBatch(dto, param);
                return Json(OperationResultWithData<IEnumerable<CalendarEventDTO>>.Success(result));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateBatchEvents", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public ActionResult GetEvents(int meetingId, string lmsProviderName)
        {
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                var result = calendarEventService.GetEvents(meetingId);
                return Json(OperationResultWithData<IEnumerable<CalendarEventDTO>>.Success(result));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetEvents", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public ActionResult CreateEvent(int meetingId, string lmsProviderName)
        {
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                LtiParamDTO param = session.LtiSession.LtiParam;
                var eve = calendarEventService.CreateEvent(meetingId, param);
                return Json(OperationResultWithData<CalendarEventDTO>.Success(eve));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateEvent", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public ActionResult SaveEvent(CalendarEventDTO ev, int meetingId, string lmsProviderName)
        {
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                LtiParamDTO param = session.LtiSession.LtiParam;
                var eve = calendarEventService.SaveEvent(meetingId, ev, param);
                return Json(OperationResultWithData<CalendarEventDTO>.Success(eve));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveEvent", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public ActionResult DeleteEvent(int meetingId, string eventId, string lmsProviderName)
        {
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                LtiParamDTO param = session.LtiSession.LtiParam;
                calendarEventService.DeleteEvent(meetingId, eventId, param);
                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteEvent", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
    }
}
