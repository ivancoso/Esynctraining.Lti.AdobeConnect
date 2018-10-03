using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("users")]
    public class UsersController : BaseApiController
    {

        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _userService;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly LmsUserServiceFactory _lmsUserServiceFactory;

        public UsersController(ApplicationSettingsProvider settings, 
            ILogger logger, 
            ZoomMeetingService meetingService,
            ILmsLicenseAccessor licenseAccessor,
            LmsUserServiceFactory lmsUserServiceFactory,
            ZoomUserService userService) : base(settings, logger)
        {
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _lmsUserServiceFactory = lmsUserServiceFactory ?? throw new ArgumentNullException(nameof(lmsUserServiceFactory));
        }

        [Route("/meetings/{meetgingId}/registrants")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<List<ZoomMeetingRegistrantDto>>> GetMeetingRegistrants(int meetgingId, [FromQuery] ZoomMeetingRegistrantStatus status)
        {
            LmsCourseMeeting meeting = await _meetingService.GetMeeting(meetgingId, CourseId);
            var registrants = _userService.GetMeetingRegistrants(meeting.ProviderMeetingId, null, status);
            return registrants.ToSuccessResult();
        }

        [Route("/meetings/{meetgingId}/syncparticipants")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<SyncParticipantsDto>> GetSyncParticipants(int meetgingId)
        {
            LmsCourseMeeting meeting = await _meetingService.GetMeeting(meetgingId, CourseId);
            var registrants = _userService.GetMeetingRegistrants(meeting.ProviderMeetingId, null, ZoomMeetingRegistrantStatus.Approved);

            var lmsSettings = LmsLicense.GetLMSSettings(Session);
            var licenseDto = await _licenseAccessor.GetLicense();
            var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);
            var lmsUsers = await lmsService.GetUsers(lmsSettings, CourseId);

            var lmsAvailableUsers = lmsUsers.Data.Where(lmsUser => !registrants.Any(r => string.Equals(r.Email, lmsUser.Email))).ToList();

            var syncParticipants = new SyncParticipantsDto
            {
                MeetingRegistants = registrants,
                LmsAvailableUsers = lmsAvailableUsers
            };

            return syncParticipants.ToSuccessResult();
        }


    }
}