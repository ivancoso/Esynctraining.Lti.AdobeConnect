namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    
    public partial class LtiController
    {
        public virtual JsonResult GetAttendanceReport(string lmsProviderName, int meetingId, int startIndex = 0, int limit = 0)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, meetingId);

                List<ACSessionParticipantDTO> report = this.meetingSetup.GetAttendanceReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                return Json(OperationResult.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
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
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                
                LmsCourseMeeting meeting = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, session.LtiSession.LtiParam.course_id, meetingId);

                List<ACSessionDTO> report = this.meetingSetup.GetSessionsReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                return Json(OperationResult.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessionsReport", credentials, ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

    }

}