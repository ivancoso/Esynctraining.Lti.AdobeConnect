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
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Logging;

    [Route("meeting/reports")]
    public partial class ReportsController : BaseApiController
    {
        [DataContract]
        public class ReportRequestDto : MeetingRequestDto
        {
            [DataMember]
            public int StartIndex { get; set; }

            [DataMember]
            public int Limit { get; set; }

        }

        private readonly IReportsService _reportService;


        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();


        public ReportsController(
            IReportsService reportService,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            _reportService = reportService;
        }


        [Route("by-attendance")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<ACSessionParticipantDto>> GetAttendanceReport([FromBody]ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionParticipantDto> report = _reportService.GetAttendanceReports(
                    meeting.ScoId,
                    this.GetAdminProvider(),
                    TimeZoneInfo.Utc,
                    request.StartIndex,
                    request.Limit);

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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<ACSessionDto>> GetSessionsReport([FromBody]ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionDto> report = _reportService.GetSessionsReports(
                    meeting.ScoId,
                    GetAdminProvider(),
                    TimeZoneInfo.Utc,
                    request.StartIndex,
                    request.Limit);

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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<RecordingTransactionDTO>> GetRecordingsReport([FromBody]ReportRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(Messages.MeetingNotFound);

                IEnumerable<RecordingTransactionDTO> report = new LtiReportService().GetRecordingsReport(
                    GetAdminProvider(),
                    meeting,
                    request.StartIndex,
                    request.Limit);

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