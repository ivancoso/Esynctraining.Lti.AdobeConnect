using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Host.Filters;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("users")]
    public class UsersController : BaseApiController
    {

        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _zoomUserService;
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
            _zoomUserService = userService ?? throw new ArgumentNullException(nameof(userService));
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
            _lmsUserServiceFactory = lmsUserServiceFactory ?? throw new ArgumentNullException(nameof(lmsUserServiceFactory));
        }

        [Route("/meetings/{meetgingId}/registrants")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<List<ZoomMeetingRegistrantDto>>> GetMeetingRegistrants(int meetgingId, [FromQuery] ZoomMeetingRegistrantStatus status)
        {
            LmsCourseMeeting meeting = await _meetingService.GetMeeting(meetgingId, CourseId);
            var registrants = await _zoomUserService.GetMeetingRegistrants(meeting, null, status);
            return registrants.ToSuccessResult();
        }

        [Route("/meetings/{meetgingId}/syncparticipants")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<SyncParticipantsDto>> GetSyncParticipants(int meetgingId)
        {
            LmsCourseMeeting meeting = await _meetingService.GetMeeting(meetgingId, CourseId);
            var registrants = (await _zoomUserService.GetMeetingRegistrants(meeting, null, ZoomMeetingRegistrantStatus.Approved))
                .Select(r => new RegistrantDto
                            {
                                Email = r.Email,
                                FirstName = r.FirstName,
                                LastName = r.LastName
                            }).ToList();

            var lmsSettings = LmsLicense.GetLMSSettings(Session);
            var licenseDto = await _licenseAccessor.GetLicense();
            var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);
            var lmsUsers = await lmsService.GetUsers(lmsSettings, CourseId);

            var zoomActiveUsers = await _zoomUserService.GetActiveUsers();

            var lmsAvailableUsers = lmsUsers.Data.Where(lmsUser => (!string.IsNullOrEmpty(lmsUser.Email)
                                                                    && !registrants.Any(r => string.Equals(r.Email, lmsUser.Email)) 
                                                                    && !lmsUser.Email.Equals(Param.lis_person_contact_email_primary, StringComparison.InvariantCultureIgnoreCase)
                                                                    && zoomActiveUsers.Any(au => au.Email.Equals(lmsUser.Email, StringComparison.InvariantCultureIgnoreCase))
                                                                    ))
                .Select(u => new LmsAvailableUserDto
            {
                    Email = u.Email,
                    FirstName = u.GetFirstName(),
                    LastName = u.GetLastName()
            }).ToList();

            var syncParticipants = new SyncParticipantsDto
            {
                MeetingRegistants = registrants,
                LmsAvailableUsers = lmsAvailableUsers
            };

            return syncParticipants.ToSuccessResult();
        }

        [Route("/meetings/{meetgingId}/registrants")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> UpdateRegistrantStatus(int meetgingId)
        {
            LmsCourseMeeting meeting = await _meetingService.GetMeeting(meetgingId, CourseId);
            await _zoomUserService.UpdateRegistrantStatus(meeting.ProviderMeetingId, new [] {Session.Email}, "deny");

            return OperationResult.Success();
        }

        [Route("")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<UserInfoDto>> GetUserInfo()
        {
            UserInfoDto user = null;
            try
            {
                bool enableSubAccounts = LmsLicense.GetSetting<bool>(LmsLicenseSettingNames.EnableSubAccounts);
                user = await _zoomUserService.GetUser(Param.lis_person_contact_email_primary, enableSubAccounts);
            }
            catch (Exception e)
            {
                Logger.Error($"User {Param.lis_person_contact_email_primary} doesn't exist or doesn't belong to this account", e);
                await _zoomUserService.CreateUser(new CreateUserDto
                {
                    Email = Param.lis_person_contact_email_primary,
                    FirstName = Param.PersonNameGiven,
                    LastName = Param.PersonNameFamily
                });

                return OperationResultWithData<UserInfoDto>.Error(
                    "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
            }

            return user.ToSuccessResult();
        }
    }
}