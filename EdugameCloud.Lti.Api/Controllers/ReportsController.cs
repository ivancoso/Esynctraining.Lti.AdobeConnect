namespace EdugameCloud.Lti.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using API.AdobeConnect;
    using Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.AdobeConnect.Api.MeetingReports;
    using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Resources;

    [Route("meeting/reports")]
    public partial class ReportsController : BaseApiController
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


        //public ReportsController(
        //    IReportsService reportService,
        //    LmsUserSessionModel userSessionModel,
        //    API.AdobeConnect.IAdobeConnectAccountService acAccountService,
        //    ApplicationSettingsProvider settings,
        //    ILogger logger, ICache cache)
        //    : base(userSessionModel, acAccountService, settings, logger, cache)
        //{
        //    _reportService = reportService;
        //}


        [Route("by-attendance")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<ACSessionParticipantDto>> GetAttendanceReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionParticipantDto> report = _reportService.GetAttendanceReports(
                    meeting.ScoId,
                    this.GetAdminProvider(),
                    TimeZoneInfo.Utc,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAttendanceReport", ex);
                return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(errorMessage);
            }
        }

        [Route("by-sessions")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<ACSessionDto>> GetSessionsReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionDto> report = _reportService.GetSessionsReports(
                    meeting.ScoId,
                    GetAdminProvider(),
                    TimeZoneInfo.Utc,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessionsReport", ex);
                return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(errorMessage);
            }
        }

        [Route("by-recordings")]
        [HttpPost]
        public virtual OperationResultWithData<IEnumerable<RecordingTransactionDTO>> GetRecordingsReport(ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.meetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(Messages.MeetingNotFound);

                IEnumerable<RecordingTransactionDTO> report = new LtiReportService().GetRecordingsReport(
                    GetAdminProvider(),
                    meeting,
                    request.startIndex,
                    request.limit);

                return report.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingsReport", ex);
                return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(errorMessage);
            }
        }

    }

}