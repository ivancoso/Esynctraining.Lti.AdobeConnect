using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Host.Filters;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Dto.OfficeHours;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("office-hours")]
    public class OfficeHoursSchedulingController : BaseApiController
    {
        private readonly ZoomOfficeHoursService _officeHoursService;
        private readonly ZoomMeetingService _meetingService;
//        private readonly ZoomMeetingApiService _meetingApiService;
//        private readonly ZoomUserService _userService;
//        private readonly IBackgroundTaskQueue _queue;
//        private readonly INotificationService _notificationService;

        public OfficeHoursSchedulingController(ApplicationSettingsProvider settings, ILogger logger,
            ZoomMeetingService meetingService, ZoomOfficeHoursService officeHoursService
            //, IBackgroundTaskQueue queue, INotificationService notificationService, ZoomMeetingApiService meetingApiService, ZoomUserService userService
            )
            : base(settings, logger)
        {
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(officeHoursService));
            _officeHoursService = officeHoursService ?? throw new ArgumentNullException(nameof(officeHoursService));
            //_queue = queue ?? throw new ArgumentNullException(nameof(queue));
            //_notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            //_meetingApiService = meetingApiService ?? throw new ArgumentNullException(nameof(meetingApiService));
            //_userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [Route("{meetingId}/availabilities")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<OfficeHoursTeacherAvailabilityDto>>> GetAvailabilities(int meetingId)
        {
            var availabilities = await _officeHoursService.GetAvailabilities(meetingId, ""); //todo: show slots accross the courses
            return availabilities.ToSuccessResult();
        }

        [Route("{meetingId}/availabilities")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailabitily(int meetingId, [FromBody]OfficeHoursTeacherAvailabilityDto dto)
        {
            //if (dto.DaysOfWeek == null || dto.DaysOfWeek.Length == 0)
            //{
            //    return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error(
            //        "Check Available Days.");
            //}
            // check isTeacher
            var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (meeting == null)
                return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error("Meeting not found");
            var result = await _officeHoursService.AddAvailability(meeting, Session.LmsUserId, dto);
            return result;
        }

        [Route("{meetingId}/slots")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> GetSlots(int meetingId, [FromQuery]long dateStart, [FromQuery]long? dateEnd = null, [FromQuery]int? status = null)
        {
            var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (meeting == null)
                return OperationResultWithData<IEnumerable<SlotDto>>.Error("Meeting not found");
            var dateStartConverted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dateStart);
            DateTime? dateEndConverted = dateEnd.HasValue ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dateEnd.Value) : (DateTime?)null;
            var slots = await _officeHoursService.GetSlots(meetingId, Session.LmsUserId, dateStartConverted, dateEndConverted);
            if (status.HasValue)
            {
                slots = slots.Where(x => x.Status == status.Value);
            }

            return slots.ToSuccessResult();
        }

        [Route("{meetingId}/slots")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> BookSlot(int meetingId, [FromBody]CreateSlotDto dto)
        {
            var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (meeting == null)
                return OperationResultWithData<SlotDto>.Error("Meeting not found");

            var slotResult = await _officeHoursService.AddSlots(meetingId, Session.LmsUserId, User.Name, new[]{dto}, status: 1);
            //if (slotResult.IsSuccess)
            //{
            //    var details = await _meetingApiService.GetMeetingApiDetails(meeting);
            //    var host = _userService.GetUser(meeting.ProviderHostId); //todo: get user data from lmsUser table
            //    if (host != null)
            //    {
            //        _queue.QueueBackgroundWorkItem(async token =>
            //        {
            //            await _notificationService.SendOHBookSlotEmail(slotResult.Data.First(), details.Topic, host.Email,
            //                $"{host.FirstName} {host.LastName}");
            //            Logger.Info($"Email for slotId={slotResult.Data.First().Id} is sent to host {host.Email}");
            //        });
            //    }
            //}

            return slotResult.IsSuccess ? slotResult.Data.First().ToSuccessResult() : OperationResultWithData<SlotDto>.Error(slotResult.Message);
        }

        [Route("slots/{slotId}/reschedule")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot(int slotId, [FromBody]RescheduleSlotDto dto)
        {
            var slot = await _officeHoursService.RescheduleSlot(slotId, Session.LmsUserId, dto);
            return slot;
        }

        [Route("{meetingId}/slots/reschedule-date")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<List<SlotDto>>> RescheduleDate(int meetingId, [FromBody]RescheduleDateDto dto)
        {
            var slots = await _officeHoursService.RescheduleDate(meetingId, Session.LmsUserId, dto);
            return slots;
        }

        [Route("slots/{slotId}/deny")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> DenySlot(int slotId, [FromBody]DenySlotDto dto)
        {
            switch (dto.Status)
            {
                case 0:
                    return await _officeHoursService.DeleteSlot(slotId, Session.LmsUserId, dto.Message);
                case 2:
                    return await _officeHoursService.UpdateSlotStatus(slotId, 2, Session.LmsUserId, dto.Message);
            }
            
            return OperationResult.Error("Deny status is not supported");
        }

        [Route("{meetingId}/slots/deny")]
        [HttpPut]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<SlotDto>> DenySlotByDate(int meetingId, [FromBody]DenySlotByDateDto dto)
        {
            var result = await _officeHoursService.DenySlotByDate(meetingId, dto.Start, Session.LmsUserId);
            return result;
        }

        [Route("{meetingId}/slots/deny-date")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> DenyDate(int meetingId, [FromBody]DenyDateDto dto)
        {
            //var result = await _officeHoursService.ResetDeniedSlots(meetingId, dto, Session.LmsUserId);
            var result = await _officeHoursService.DeleteSlots(meetingId, dto, Session.LmsUserId);
            return result;
        }

        [Route("{meetingId}/slots/reset-date")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> ResetDeniedDate(int meetingId, [FromBody]DenyDateDto dto)
        {
            var result = await _officeHoursService.ResetDeniedSlots(meetingId, dto, Session.LmsUserId);
            return result;
        }

    }
}