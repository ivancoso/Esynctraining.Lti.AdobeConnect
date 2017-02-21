namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Web.Http;
    using API.AdobeConnect;
    using Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.AdobeConnect.Api.MeetingReports;
    using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Resources;

    public partial class LtiReportController : BaseApiController
    {
        [DataContract]
        public class ReportRequestDto : MeetingRequestDto
        {
            [DataMember]
            public int startIndex { get; set; }

            [DataMember]
            public int limit { get; set; }

        }

        private readonly IReportsService _reportService;


        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();


        public LtiReportController(
            IReportsService reportService,
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            _reportService = reportService;
        }


        [Route("meeting/attendance")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<ACSessionParticipantDto>> GetAttendanceReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionParticipantDto> report = _reportService.GetAttendanceReports(
                    meeting.ScoId,
                    this.GetAdobeConnectProvider(credentials),
                    TimeZoneInfo.Utc,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAttendanceReport", credentials, ex);
                return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(errorMessage);
            }
        }

        [Route("meeting/sessions")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<ACSessionDto>> GetSessionsReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;
                
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionDto> report = _reportService.GetSessionsReports(
                    meeting.ScoId,
                    this.GetAdobeConnectProvider(credentials),
                    TimeZoneInfo.Utc,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessionsReport", credentials, ex);
                return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(errorMessage);
            }
        }

        [Route("meeting/reports/by-recordings")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<RecordingTransactionDTO>> GetRecordingsReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(request.lmsProviderName);
                credentials = session.LmsCompany;

                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(Messages.MeetingNotFound);

                IEnumerable<RecordingTransactionDTO> report = new LtiReportService().GetRecordingsReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingsReport", credentials, ex);
                return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(errorMessage);
            }
        }

    }

}