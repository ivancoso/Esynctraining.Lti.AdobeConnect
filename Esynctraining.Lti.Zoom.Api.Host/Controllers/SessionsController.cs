using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("")]
    public class SessionsController : BaseApiController
    {
        private readonly IMeetingSessionService _meetingSessionService;
        private readonly ZoomMeetingService _meetingService;

        public SessionsController(ApplicationSettingsProvider settings, ILogger logger, ZoomMeetingService meetingService, 
            IMeetingSessionService meetingSessionService)
            : base(settings, logger)
        {
            _meetingService = meetingService;
            _meetingSessionService = meetingSessionService;
        }

        [Route("meetings/{meetingId}/sessions/createBatch")]
        [HttpPost]
        [LmsAuthorizeBase]
        //[TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<IEnumerable<MeetingSessionDto>>> CreateBatch(int meetingId, [FromBody]CreateMeetingSessionsBatchDto dto)
        {
            try
            {
                var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (meeting == null)
                {
                    //log
                    return OperationResultWithData<IEnumerable<MeetingSessionDto>>.Error("Meeting not found or cannot be accessed with current session");
                }
                var result = await _meetingSessionService.CreateBatchAsync(dto, meeting);
                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateBatchEvents", ex);

                return OperationResultWithData<IEnumerable<MeetingSessionDto>>.Error(errorMessage);
            }
        }

        [Route("meetings/{meetingId}/sessions")]
        [HttpGet]
        [LmsAuthorizeBase]
        //[TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<IEnumerable<MeetingSessionDto>>> GetSessions(int meetingId)
        {
            try
            {
                var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (meeting == null)
                {
                    //log
                    return OperationResultWithData<IEnumerable<MeetingSessionDto>>.Error("Meeting not found or cannot be accessed with current session");
                }
                var result = await _meetingSessionService.GetSessions(meeting);
                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessions", ex);
                return OperationResultWithData<IEnumerable<MeetingSessionDto>>.Error(errorMessage);
            }
        }

        [Route("meetings/{meetingId}/sessions")]
        [HttpPost]
        [LmsAuthorizeBase]
        //[TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<MeetingSessionDto>> CreateSession(int meetingId)
        {
            try
            {
                var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (meeting == null)
                {
                    //log
                    return OperationResultWithData<MeetingSessionDto>.Error("Meeting not found or cannot be accessed with current session");
                }

                var session = await _meetingSessionService.CreateSessionAsync(meeting);
                return session.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateSession", ex);
                return OperationResultWithData<MeetingSessionDto>.Error(errorMessage);
            }
        }

        [Route("meetings/{meetingId}/sessions/{sessionId}")]
        [HttpPut]
        [LmsAuthorizeBase]
        //[TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResultWithData<MeetingSessionDto>> SaveEvent(int meetingId, int sessionId, [FromBody]MeetingSessionUpdateDto model)
        {
            try
            {
                var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (meeting == null)
                {
                    //log
                    return OperationResultWithData<MeetingSessionDto>.Error("Meeting not found or cannot be accessed with current session");
                }

                var eve = await _meetingSessionService.SaveSessionAsync(meeting, sessionId, model);
                return eve.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSession", ex);
                return OperationResultWithData<MeetingSessionDto>.Error(errorMessage);
            }
        }

        [Route("meetings/{meetingId}/sessions/{sessionId}")]
        [HttpDelete]
        [LmsAuthorizeBase]
        //[TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMeetingSessions)]
        public async Task<OperationResult> DeleteEvent(int meetingId, int? sessionId)
        {
            try
            {
                var meeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (meeting == null)
                {
                    //log
                    return OperationResultWithData<MeetingSessionDto>.Error("Meeting not found or cannot be accessed with current session");
                }

                await _meetingSessionService.DeleteSessionAsync(meeting, sessionId.GetValueOrDefault());
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteSession", ex);
                return OperationResult.Error(errorMessage);
            }
        }

    }
}