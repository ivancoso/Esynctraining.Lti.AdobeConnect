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
        private readonly IOfficeHoursService _officeHoursService;

        public OfficeHoursSchedulingController(
            //MeetingSetup meetingSetup,
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache, IJsonSerializer jsonSerializer,
            ZoomUserService userService, ZoomRecordingService recordingService, ZoomMeetingService meetingService)
            : base(settings, logger)
        {
            //_meetingSetup = meetingSetup;
            //_jsonSerializer = jsonSerializer;
            //_lmsCourseMeetingModel = lmsCourseMeetingModel;
            //_userSessionModel = userSessionModel;
            //_userService = userService;
            //_meetingService = meetingService;
            //_lmsFactory = lmsFactory;
        }


        [Route("{meetingId}/availability")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<OfficeHoursTeacherAvailabilityDto>> GetAvailabitily(int meetingId)
        {

            return new OfficeHoursTeacherAvailabilityDto
            {
                Duration = 15,
                PeriodStart = DateTime.Today,
                PeriodEnd = DateTime.Today.AddMonths(2),
                DaysOfWeek = new []{2, 4},
                Intervals = new List<AvailabilityInterval> {new AvailabilityInterval { Start = 8*60, End = (8+2)*60} }
            }.ToSuccessResult();
        }

        [Route("{meetingId}/availability")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> AddAvailabitily(int meetingId, [FromBody]OfficeHoursTeacherAvailabilityDto dto)
        {
            // check isTeacher
            //_officeHoursService.AddAvailability()
            return OperationResult.Success();
        }

        [Route("{meetingId}/slots")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<List<SlotDto>>> GetSlots(int meetingId, [FromQuery]long dateStart)
        {
            var date = new DateTime(1970, 1, 1, 0,0,0, DateTimeKind.Utc).AddMilliseconds(dateStart);
            return new List<SlotDto>
            {
                new SlotDto(){Status = 1, Start = date, End = date.AddMinutes(30)},
                new SlotDto(){Start = date.AddMinutes(60), End = date.AddMinutes(90)}
            }.ToSuccessResult();
        }

        [Route("{meetingId}/slots")]
        [HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<SlotDto>> BookSlot(int meetingId, [FromBody]CreateSlotDto dto)
        {
            return new SlotDto{Start = dto.Start, Status = 1, End = dto.End, UserName = "Test", Subject = dto.Subject, Questions = dto.Questions}.ToSuccessResult();
        }

        [Route("slots/{slotId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> CancelSlot(int slotId)
        {
            return OperationResult.Success();
        }

        [Route("slots/{slotId}/status/{status}")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> MarkSlotUnavailable(int slotId, int status)
        {
            return OperationResult.Success();
        }
    }

    public class MarkSlotUnavailableDto
    {
        public int Status { get; set; }
    }

    public class GetSlotsDto
    {
        public DateTime DateStart { get; set; }
    }

    public class SlotDto : CreateSlotDto
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string UserName { get; set; }
    }

    public class CreateSlotDto
    {
        /// <summary>
        /// 0 - Free, 1 - Booked, 2 - NotAvailable
        /// </summary>
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}