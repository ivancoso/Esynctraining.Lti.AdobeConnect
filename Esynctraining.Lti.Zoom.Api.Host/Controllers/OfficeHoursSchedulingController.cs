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

        [Route("{meetingId}/availability")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> GetAvailabitily(int meetingId)
        {
            var availability = await _officeHoursService.GetAvailability(meetingId, 0, ""); //todo: show slots accross the courses
            return availability.ToSuccessResult();
        }

        [Route("{meetingId}/availability")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailabitily(int meetingId, [FromBody]OfficeHoursTeacherAvailabilityDto dto)
        {
            // check isTeacher
            var meeting = await _meetingService.GetMeeting(meetingId, Session.LicenseId, CourseId);
            var result = await _officeHoursService.AddAvailability(meeting, Session.LmsUserId, dto);
            return result.ToSuccessResult();
        }

        [Route("{meetingId}/slots")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> GetSlots(int meetingId, [FromQuery]long dateStart)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dateStart);
            var slots = await _officeHoursService.GetSlots(meetingId, date, Session.LmsUserId);
            return slots.ToSuccessResult();
        }

        [Route("{meetingId}/slots")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> BookSlot(int meetingId, [FromBody]CreateSlotDto dto)
        {
            var slot = await _officeHoursService.AddSlot(meetingId, Session.LmsUserId, Session.Email, dto, status: 1);
            return slot.ToSuccessResult();
        }

        [Route("slots/{slotId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> CancelSlot(int slotId)
        {
            var result = await _officeHoursService.CancelSlot(slotId, Session.LmsUserId);
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