using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO.OfficeHours;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("office-hours")]
    public class OfficeHoursSchedulingController : BaseApiController
    {
        private readonly OfficeHoursService _officeHoursService;
        //        private readonly IBackgroundTaskQueue _queue;
        //        private readonly INotificationService _notificationService;
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;

        public OfficeHoursSchedulingController(API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache,
            LmsCourseMeetingModel lmsCourseMeetingModel, OfficeHoursService officeHoursService
            //, IBackgroundTaskQueue queue, INotificationService notificationService
        ) : base(acAccountService, settings, logger, cache)
        {
            _officeHoursService = officeHoursService ?? throw new ArgumentNullException(nameof(officeHoursService));
            //_queue = queue ?? throw new ArgumentNullException(nameof(queue));
            //_notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _lmsCourseMeetingModel = lmsCourseMeetingModel ?? throw new ArgumentNullException(nameof(lmsCourseMeetingModel));
        }

        [Route("availabilities")]
        [HttpPost]
        [LmsAuthorizeBase]
        public async Task<OperationResultWithData<IEnumerable<OfficeHoursTeacherAvailabilityDto>>> GetAvailabilities([FromBody]MeetingIdDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<IEnumerable<OfficeHoursTeacherAvailabilityDto>>.Error("Meeting not found");
            var availabilities = await _officeHoursService.GetAvailabilities(meeting.OfficeHours.Id, null); //todo: show slots accross the courses
            return availabilities.ToSuccessResult();
        }

        [Route("availabilities/add")]
        [HttpPost]
        [TeacherOnly]
        public async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> AddAvailabitily([FromBody]OfficeHoursTeacherAvailabilityDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<OfficeHoursTeacherAvailabilityDto>.Error("Meeting not found");
            var result = await _officeHoursService.AddAvailability(meeting.OfficeHours, Session.LmsUser, dto);
            return result;
        }

        [Route("slots")]
        [HttpPost]
        [LmsAuthorizeBase]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> GetSlots([FromBody]GetSlotsDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<IEnumerable<SlotDto>>.Error("Meeting not found");
            var dateStartConverted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dto.Start);
            DateTime? dateEndConverted = dto.End.HasValue ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(dto.End.Value) : (DateTime?)null;
            var slots = await _officeHoursService.GetSlots(meeting.OfficeHours.Id, Session.LmsUser.Id, dateStartConverted, dateEndConverted);
            if (dto.Status.HasValue)
            {
                slots = slots.Where(x => x.Status == dto.Status.Value);
            }

            return slots.ToSuccessResult();
        }

        [Route("slots/book")]
        [HttpPost]
        [LmsAuthorizeBase]
        public async Task<OperationResultWithData<SlotDto>> BookSlot([FromBody]CreateSlotDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<SlotDto>.Error("Meeting not found");

            var slotResult = await _officeHoursService.AddSlots(meeting.OfficeHours.Id, Session.LmsUser, new[] { dto }, status: OfficeHoursSlotStatus.Booked);
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

        [Route("slots/reschedule")]
        [HttpPost]
        [TeacherOnly]
        public async Task<OperationResultWithData<SlotDto>> RescheduleSlot([FromBody]RescheduleSlotDto dto)
        {
            var slot = await _officeHoursService.RescheduleSlot(dto.SlotId, Session.LmsUser, dto);
            return slot;
        }

        [Route("slots/reschedule-date")]
        [HttpPost]
        [TeacherOnly]
        public async Task<OperationResultWithData<List<SlotDto>>> RescheduleDate([FromBody]RescheduleDateDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<List<SlotDto>>.Error("Meeting not found");
            var slots = await _officeHoursService.RescheduleDate(meeting.OfficeHours.Id, Session.LmsUser, dto);
            return slots;
        }

        [Route("slots/deny")]
        [HttpPost]
        [LmsAuthorizeBase]
        public async Task<OperationResult> DenySlot([FromBody]DenySlotDto dto)
        {
            switch (dto.Status)
            {
                case 0:
                    return await _officeHoursService.DeleteSlot(dto.SlotId, Session.LmsUser, dto.Message);
                case 2:
                    return await _officeHoursService.UpdateSlotStatus(dto.SlotId, OfficeHoursSlotStatus.Cancelled, Session.LmsUser, dto.Message);
            }

            return OperationResult.Error("Deny status is not supported");
        }

        [Route("slots/deny-by-date")]
        [HttpPost]
        [LmsAuthorizeBase] // TeacherOnly ?
        public async Task<OperationResultWithData<SlotDto>> DenySlotByDate([FromBody]DenySlotByDateDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<SlotDto>.Error("Meeting not found");
            var result = await _officeHoursService.DenySlotByDate(meeting.OfficeHours.Id, dto.Start, Session.LmsUser);
            return result;
        }

        [Route("slots/deny-date")]
        [HttpPost]
        [TeacherOnly]
        public async Task<OperationResultWithData<IEnumerable<SlotDto>>> DenyDate([FromBody]DenyDateDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<IEnumerable<SlotDto>>.Error("Meeting not found");
            var result = await _officeHoursService.DeleteSlots(meeting.OfficeHours.Id, dto, Session.LmsUser);
            return result;
        }

        [Route("slots/reset-date")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResult> ResetDeniedDate([FromBody]DenyDateDto dto)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, dto.MeetingId);
            if (meeting?.OfficeHours == null)
                return OperationResultWithData<IEnumerable<SlotDto>>.Error("Meeting not found");
            var result = await _officeHoursService.ResetDeniedSlots(meeting.OfficeHours.Id, dto);
            return result;
        }
    }
}