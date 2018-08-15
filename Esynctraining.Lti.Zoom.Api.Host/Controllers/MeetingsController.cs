using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("meetings")]
    public class MeetingsController : BaseApiController
    {
        private readonly ZoomUserService _userService;

        private readonly ZoomMeetingService _meetingService;
        //private readonly LmsFactory _lmsFactory;

        #region Constructors and Destructors

        public MeetingsController(
            ApplicationSettingsProvider settings,
            ILogger logger, IJsonSerializer jsonSerializer,
            ZoomUserService userService, ZoomRecordingService recordingService, ZoomMeetingService meetingService)
            : base(settings, logger)
        {
            _userService = userService;
            _meetingService = meetingService;
        }

        #endregion

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<MeetingViewModel>>> GetCourseMeetings()
        {
            StringBuilder trace = null;

            string userId = null;
            try
            {
                var user = _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
            }
            //if (!string.IsNullOrEmpty(userId))
            //{

            var zoomMeetings = await _meetingService.GetMeetings(CourseId, userId);
            return zoomMeetings.ToSuccessResult();
            //}

            //return OperationResultWithData<IEnumerable<MeetingViewModel>>.Error("User does not exist in Zoom.");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingDetailsViewModel>> GetMeetingDetails(int meetingId)
        {
            StringBuilder trace = null;

            try
            {
                var viewModel = await _meetingService.GetMeetingDetails(meetingId, CourseId.ToString());
                return viewModel.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Details error.", e);
            }
            return OperationResultWithData<MeetingDetailsViewModel>.Error("Unexpected error happened");
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingViewModel>> Create(
            [FromBody] CreateMeetingViewModel requestDto)
        {
            OperationResultWithData<MeetingViewModel> result = null;
            try
            {
                string userId = null;
                UserInfoDto user = null;
                try
                {
                    user = _userService.GetUser(Param.lis_person_contact_email_primary);
                    userId = user.Id;

                    if (!IsPossibleCreateMeeting(userId, requestDto, out string errorMessage))
                    {
                        return OperationResultWithData<MeetingViewModel>.Error(errorMessage);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("User doesn't exist or doesn't belong to this account", e);
                    /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                    var userInfo = _userService.CreateUser(new CreateUserDto
                    {
                        Email = Param.lis_person_contact_email_primary,
                        FirstName = Param.PersonNameGiven,
                        LastName = Param.PersonNameFamily
                    });

                    return OperationResultWithData<MeetingViewModel>.Error(
                        "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    var licenseSettings = LmsLicense.GetLMSSettings(Session);
                    var createResult = await _meetingService.CreateMeeting(licenseSettings, CourseId.ToString(),
                        user,
                        Param.lis_person_contact_email_primary, requestDto);

                    return createResult;
                }

                return OperationResultWithData<MeetingViewModel>.Error("User does not exist in Zoom.");
            }
            catch (ZoomLicenseException ex)
            {
                throw;
            }
            catch (ZoomApiException ex)
            {
                Logger.Error(ex.ErrorMessage, ex);
                return OperationResultWithData<MeetingViewModel>.Error(ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateMeeting", ex);
                return OperationResultWithData<MeetingViewModel>.Error(errorMessage);
            }

        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> Update([FromBody] CreateMeetingViewModel vm,
            [FromRoute] int meetingId)
        {
            UserInfoDto user = null;
            try
            {
                user = _userService.GetUser(Param.lis_person_contact_email_primary);
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
                /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                var userInfo = _userService.CreateUser(new CreateUserDto
                {
                    Email = Param.lis_person_contact_email_primary,
                    FirstName = Param.PersonNameGiven,
                    LastName = Param.PersonNameFamily
                });

                return OperationResult.Error(
                    "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
            }
            try
            {
                var licenseSettings = LmsLicense.GetLMSSettings(Session);
                var updated = await _meetingService.UpdateMeeting(meetingId, licenseSettings, CourseId,
                    Param.lis_person_contact_email_primary, vm, user);

                return updated ? OperationResult.Success() : OperationResult.Error("Meeting has not been updated");
            }
            catch (ZoomLicenseException e)
            {
                throw;
            }
            catch (ZoomApiException ex)
            {
                Logger.Error(ex.ErrorMessage, ex);
                return OperationResultWithData<MeetingViewModel>.Error(ex.ErrorMessage);
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Update error.", e);
            }
            return OperationResult.Error("Unexpected error happened");
        }

        [Route("{meetingId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteMeeting(int meetingId, [FromQuery] bool remove = false)
        {
            //param.lis_person_contact_email_primary
            var result = await _meetingService.DeleteMeeting(meetingId, CourseId,
                Param.lis_person_contact_email_primary, remove);

            return result;

            //return OperationResult.Error("Error during delete. Please try again or contact support.");
        }

        private bool IsPossibleCreateMeeting(string userId, CreateMeetingViewModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var zoomUser = _userService.GetUser(userId);
            if (zoomUser.Type == (int)UserTypes.Basic && model.Settings.ApprovalType == (int)MeetingApprovalTypes.Manual)
            {
                errorMessage = "Basic user cannot create meeting with Enabled class roster security option.";
                return false;
            }

            return true;
        }
    }
}