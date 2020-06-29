using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
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
            ILogger logger, IJsonSerializer json, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, json, cache)
        {
        }

        #region Public Methods and Operators
                
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> JoinRecording(string session, string recordingUrl)
        {
            try
            {
                var s = GetReadOnlySession(session);
                var param = s.LtiSession?.LtiParam;

                var joinRes = await RecordingsService.JoinRecording(s.LmsCompany, param, recordingUrl, GetAdminProvider(s.LmsCompany));
                var url = joinRes.Item1;
                var breezeSession = joinRes.Item2;

                return this.LoginToAC(url, breezeSession, s.LmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("JoinRecording", session, ex);
            }
        }
        
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> EditRecording(string session, string recordingUrl)
        {
            try
            {
                var s = GetReadOnlySession(session);
                var param = s.LtiSession?.LtiParam;

                var joinRes = await RecordingsService.JoinRecording(s.LmsCompany, param, recordingUrl, GetAdminProvider(s.LmsCompany), "edit");
                var url = joinRes.url;
                var breezeSession = joinRes.breezeSession;

                return LoginToAC(url, breezeSession, s.LmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("EditRecording", session, ex);
            }
        }
        
        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> GetRecordingFlv(string session, string recordingUrl)
        {
            try
            {
                var s = GetReadOnlySession(session);
                var param = s.LtiSession?.LtiParam;

                var joinRes = await RecordingsService.JoinRecording(s.LmsCompany, param, recordingUrl, GetAdminProvider(s.LmsCompany), "offline");
                return LoginToAC(joinRes.url, joinRes.breezeSession, s.LmsCompany);
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