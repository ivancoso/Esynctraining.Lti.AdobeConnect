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
        /// The delete recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult DeleteRecording(string lmsProviderName, string scoId, string id)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.RemoveRecording(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    id,
                    scoId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteRecording", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        /// <summary>
        /// The get recordings.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName, string scoId)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                List<RecordingDTO> recordings = this.meetingSetup.GetRecordings(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    scoId);

                return Json(OperationResult.Success(recordings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        /// <summary>
        /// The join recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession);
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinRecording", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// The share recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingId">
        /// The recording id.
        /// </param>
        /// <param name="isPublic">
        /// The is public.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult ShareRecording(string lmsProviderName, string recordingId, bool isPublic, string password)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var link = this.meetingSetup.UpdateRecording(credentials, this.GetAdobeConnectProvider(credentials), recordingId, isPublic, password);

                return Json(OperationResult.Success(link));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ShareRecording", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        /// <summary>
        /// The edit recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "edit");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditRecording", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// The get recording FLV.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "offline");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingFlv", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

    }

}