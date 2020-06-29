using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Host.Filters;
using Esynctraining.Lti.Zoom.Common.Dto.Reports;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("reports")]
    public class ReportsController : BaseApiController
    {
        private readonly ZoomReportService _reportService;
        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _userService;

        public ReportsController(
            ApplicationSettingsProvider settings, ILogger logger,
            ZoomReportService reportService, ZoomMeetingService meetingService, ZoomUserService userService)
            : base(settings, logger)
        {
            _reportService = reportService;
            _meetingService = meetingService;
            _userService = userService;
        }

        [Route("meetings/{meetingId}/by-sessions")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomSessionDto>>> GetReportBySessions(int meetingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResultWithData<IEnumerable<ZoomSessionDto>>.Error("Meeting not found");
            bool enableSubAccounts = LmsLicense.GetSetting<bool>(LmsLicenseSettingNames.EnableSubAccounts);
            var zoomUser = await _userService.GetUser(dbMeeting.ProviderHostId, enableSubAccounts);

            var sessions = await _reportService.GetSessionsReport(dbMeeting, zoomUser);
            return sessions.ToSuccessResult();
        }

        [Route("participants/{sessionId}")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomSessionParticipantDto>>> GetReportParticipantsBySessions(string sessionId)
        {
            var sessions = await _reportService.GetParticipantsBySessionId(WebUtility.UrlDecode(sessionId).Replace(" ", "+"), null);
            return sessions.ToSuccessResult();
        }

    }
}