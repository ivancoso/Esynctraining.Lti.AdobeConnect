namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public partial class LtiController
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get attendance report.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult GetAttendanceReport(string lmsProviderName, string scoId, int startIndex = 0, int limit = 0)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                List<ACSessionParticipantDTO> report = this.meetingSetup.GetAttendanceReport(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    session.LtiSession.LtiParam,
                    scoId,
                    startIndex,
                    limit);

                return Json(OperationResult.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetAttendanceReport", ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// The get sessions report.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult GetSessionsReport(string lmsProviderName, string scoId, int startIndex = 0, int limit = 0)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                List<ACSessionDTO> report = this.meetingSetup.GetSessionsReport(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    scoId,
                    startIndex,
                    limit);

                return Json(OperationResult.Success(report), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetSessionsReport", ex);
                return Json(OperationResult.Error(errorMessage), this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

    }

}