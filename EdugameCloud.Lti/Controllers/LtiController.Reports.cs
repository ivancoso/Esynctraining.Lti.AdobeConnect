namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using API.AdobeConnect;
    using Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Resources;

    public partial class LtiReportController : BaseController
    {
        private readonly IReportService meetingSetup;

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }


        public LtiReportController(
            IReportService meetingSetup,
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
            this.meetingSetup = meetingSetup;
        }


        public virtual JsonResult GetAttendanceReport(string lmsProviderName, int meetingId, int startIndex = 0, int limit = 0)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, meetingId);
                if (meeting == null)
                    return Json(OperationResult.Error(Messages.MeetingNotFound));

                List<ACSessionParticipantDTO> report = this.meetingSetup.GetAttendanceReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                return Json(OperationResultWithData<List<ACSessionParticipantDTO>>.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAttendanceReport", credentials, ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

        public virtual JsonResult GetSessionsReport(string lmsProviderName, int meetingId, int startIndex = 0, int limit = 0)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, meetingId);
                if (meeting == null)
                    return Json(OperationResult.Error(Messages.MeetingNotFound));

                List<ACSessionDTO> report = this.meetingSetup.GetSessionsReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                return Json(OperationResultWithData<List<ACSessionDTO>>.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessionsReport", credentials, ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

        public virtual JsonResult GetRecordingsReport(string lmsProviderName, int meetingId, int startIndex = 0, int limit = 0)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;

                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, meetingId);
                if (meeting == null)
                    return Json(OperationResult.Error(Messages.MeetingNotFound));

                IEnumerable<RecordingTransactionDTO> report = this.meetingSetup.GetRecordingsReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                return Json(OperationResultWithData<IEnumerable<RecordingTransactionDTO>>.Success(report), 
                    this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingsReport", credentials, ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

    }

}