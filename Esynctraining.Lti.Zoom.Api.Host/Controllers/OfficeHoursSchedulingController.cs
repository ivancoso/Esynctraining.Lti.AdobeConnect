using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("office-hours")]
    public class OfficeHoursSchedulingController : BaseApiController
    {
        private readonly ZoomOfficeHoursService _officeHoursService;
        private readonly ZoomMeetingService _meetingService;

        public OfficeHoursSchedulingController(
            ApplicationSettingsProvider settings,
            ILogger logger, ZoomMeetingService meetingService, ZoomOfficeHoursService officeHoursService)
            : base(settings, logger)
        {
            
            _meetingService = meetingService;
            _officeHoursService = officeHoursService;
        }

        [Route("{meetingId}/availabilities")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<OfficeHoursTeacherAvailabilityDto>>> GetAvailabilities(int meetingId)
        {
            var availabilities = await _officeHoursService.GetAvailabilities(meetingId, 0, ""); //todo: show slots accross the courses
            return availabilities.ToSuccessResult();
        }

        [Route("{meetingId}/availabilities")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailabitily(int meetingId, [FromBody]OfficeHoursTeacherAvailabilityDto dto)
        {
            // check isTeacher
            var meeting = await _meetingService.GetMeeting(meetingId, Session.LicenseId, CourseId);
            var result = await _officeHoursService.AddAvailability(meeting, Session.LmsUserId, dto);
            return result;
        }

        [Route("{meetingId}/slots")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> GetSlots(int meetingId, [FromQuery]long dateStart, [FromQuery]long? dateEnd)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dateStart);
            DateTime? dEnd = dateEnd.HasValue ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dateEnd.Value) : (DateTime?)null;
            var slots = await _officeHoursService.GetSlots(meetingId, Session.LmsUserId, date, dEnd);
            return slots.ToSuccessResult();
        }

        [Route("{meetingId}/slots")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> BookSlot(int meetingId, [FromBody]CreateSlotDto dto)
        {
            var slotResult = await _officeHoursService.AddSlot(meetingId, Session.LmsUserId, Session.Email, dto, status: 1);
            return slotResult;
        }

        [Route("{slots/{slotId}")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot(int slotId, [FromBody]RescheduleSlotDto dto)
        {
            var slot = await _officeHoursService.RescheduleSlot(slotId, Session.LmsUserId, dto);
            return slot;
        }

        [Route("slots/{slotId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> CancelSlot(int slotId)
        {
            var result = await _officeHoursService.DeleteSlot(slotId, Session.LmsUserId);
            return result;
        }

        [Route("slots/{slotId}/status/{status}")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> MarkSlotUnavailable(int slotId, int status)
        {
            var result = await _officeHoursService.UpdateSlotStatus(slotId, status:2);
            return result;
        }
    }
}