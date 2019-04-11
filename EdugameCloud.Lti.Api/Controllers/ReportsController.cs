using EdugameCloud.Lti.Core.Business.Models;

namespace EdugameCloud.Lti.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using API.AdobeConnect;
    using EdugameCloud.Lti.Api.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.Resources;
    using Esynctraining.AdobeConnect.Api.MeetingReports;
    using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Microsoft.AspNetCore.Mvc;

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
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            _reportService = reportService;
        }


        [Route("by-attendance")]
        [HttpPost]
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<ACSessionParticipantDto>> GetAttendanceReport([FromBody]ReportRequestDto request)
        {
            try
            {
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionParticipantDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionParticipantDto> report = _reportService.GetAttendanceReports(
                    meeting.GetMeetingScoId(),
                    this.GetAdminProvider(),
                    TimeZoneInfo.Utc,
                    request.StartIndex,
                    request.Limit);

                // TRICK: check that is not API call.
                if (SessionSave != null)
                {
                    // TRICK: clean not to serialize
                    foreach (var item in report)
                    {
                        item.login = null;
                        item.principalId = null;
                    }
                }

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
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<ACSessionDto>> GetSessionsReport([FromBody]ReportRequestDto request)
        {
            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<ACSessionDto>>.Error(Messages.MeetingNotFound);

                IEnumerable<ACSessionDto> report = _reportService.GetSessionsReports(
                    meeting.GetMeetingScoId(),
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
        [Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual OperationResultWithData<IEnumerable<RecordingTransactionDTO>> GetRecordingsReport([FromBody]MeetingRequestDto request)
        {
            try
            {
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                if (meeting == null)
                    return OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Error(Messages.MeetingNotFound);

                IEnumerable<RecordingTransactionDTO> report = new LtiReportService().GetRecordingsReport(
                    GetAdminProvider(),
                    meeting);

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