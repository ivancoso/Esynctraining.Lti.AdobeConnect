using System;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiRecordingController : BaseController
    {
        private IRecordingsService RecordingsService => IoC.Resolve<IRecordingsService>();


        public LtiRecordingController(
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
        }

        #region Public Methods and Operators
                
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ActionResult JoinRecording(string session, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var s = GetReadOnlySession(session);
                lmsCompany = s.LmsCompany;
                var param = s.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdminProvider(lmsCompany);
                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider);
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("JoinRecording", session, ex);
            }
        }
        
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ActionResult EditRecording(string session, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var s = GetReadOnlySession(session);
                lmsCompany = s.LmsCompany;
                var param = s.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdminProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "edit");
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("EditRecording", session, ex);
            }
        }
        
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ActionResult GetRecordingFlv(string session, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var s = GetReadOnlySession(session);
                lmsCompany = s.LmsCompany;
                var param = s.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdminProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "offline");
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("GetRecordingFlv", session, ex);
            }
        }

        #endregion

        #region methods

        private ActionResult LoginToAC(string realUrl, string breezeSession, LmsCompany credentials)
        {
            if (!credentials.LoginUsingCookie.GetValueOrDefault())
            {
                return this.Redirect(realUrl);
            }

            this.ViewBag.MeetingUrl = realUrl;
            this.ViewBag.BreezeSession = breezeSession;
            this.ViewBag.AcServer = credentials.AcServer + "/";

            return this.View("~/Views/Lti/LoginToAC.cshtml");
        }

        //private static Recording GetScheduledRecording(string recordingScoId, string meetingScoId, Esynctraining.AdobeConnect.IAdobeConnectProxy adobeConnectProvider)
        //{
        //    var recordingsByMeeting = adobeConnectProvider.GetRecordingsList(meetingScoId);
        //    if (recordingsByMeeting == null || !recordingsByMeeting.Success || recordingsByMeeting.Values == null || !recordingsByMeeting.Values.Any() )
        //    {
        //        return null;
        //    }

        //    return recordingsByMeeting.Values.SingleOrDefault(x => x.ScoId == recordingScoId);
        //}
        
        private ActionResult RecordingsError(string method, string sessionId, Exception ex)
        {
            Logger.ErrorFormat(ex, "{0} exception. sessionId:{1}.", method, sessionId);
            this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
            return this.View("~/Views/Lti/LtiError.cshtml");
        }

        #endregion

    }

}