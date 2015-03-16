using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;
using EdugameCloud.Lti.API.Desire2Learn;
using EdugameCloud.Lti.OAuth.Desire2Learn;
using RestSharp;

namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using DotNetOpenAuth.AspNet;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.Models;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.Lti.OAuth.Canvas;
    using EdugameCloud.Lti.Utils;
    using Esynctraining.AC.Provider;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Microsoft.Web.WebPages.OAuth;

    using Newtonsoft.Json;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public class LtiController : Controller
    {
        #region Static Fields

        /// <summary>
        ///     The is debug.
        /// </summary>
        private static bool? isDebug;

        /// <summary>
        /// The company LMS model.
        /// </summary>
        private readonly CompanyLmsModel companyLmsModel;

        /// <summary>
        /// The user session model.
        /// </summary>
        private readonly LmsUserSessionModel userSessionModel;

        /// <summary>
        /// The LMS user model.
        /// </summary>
        private readonly LmsUserModel lmsUserModel;

        /// <summary>
        /// The Meeting setup.
        /// </summary>
        private readonly MeetingSetup meetingSetup;

        /// <summary>
        /// The users setup.
        /// </summary>
        private readonly UsersSetup usersSetup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LtiController"/> class.
        /// </summary>
        /// <param name="companyLmsModel">
        /// Company LMS model
        /// </param>
        /// <param name="userSessionModel">
        /// The user Session Model.
        /// </param>
        /// <param name="lmsUserModel">
        /// LMS user model
        /// </param>
        /// <param name="meetingSetup">
        /// The meeting Setup.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="usersSetup">
        /// The users setup.
        /// </param>
        public LtiController(
            CompanyLmsModel companyLmsModel,
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup)
        {
            this.companyLmsModel = companyLmsModel;
            this.userSessionModel = userSessionModel;
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.Settings = settings;
            this.usersSetup = usersSetup;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the settings.
        /// </summary>
        public dynamic Settings { get; private set; }

        /// <summary>
        ///     Gets the company LMS model.
        /// </summary>
        private CompanyLmsModel CompanyLmsModel
        {
            get
            {
                return this.companyLmsModel;
            }
        }

        /// <summary>
        ///     Gets the LMS user model.
        /// </summary>
        private LmsUserModel LmsUserModel
        {
            get
            {
                return this.lmsUserModel;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether is debug.
        /// </summary>
        private bool IsDebug
        {
            get
            {
                if (isDebug.HasValue)
                {
                    return isDebug.Value;
                }

                bool val;
                isDebug = bool.TryParse(this.Settings.IsDebug, out val) && val;
                return isDebug.Value;
            }
        }

        /// <summary>
        ///     Gets the meeting setup.
        /// </summary>
        private MeetingSetup MeetingSetup
        {
            get
            {
                return this.meetingSetup;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="__provider__">
        /// The __provider__.
        /// </param>
        /// <param name="__sid__">
        /// The __sid__.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here."), ActionName("callback")]
        [AllowAnonymous]
        public virtual ActionResult AuthenticationCallback(
            // ReSharper disable once InconsistentNaming
            string __provider__, 
            // ReSharper disable once InconsistentNaming
            string __sid__ = null, 
            string code = null, 
            string state = null,
            string providerKey = null)
        {
            __provider__ = FixExtraDataIssue(__provider__);
            providerKey = FixExtraDataIssue(providerKey);
            string provider = __provider__;

            if (provider.ToLower() == LmsProviderNames.Desire2Learn)
            {
                var d2lService = IoC.Resolve<IDesire2LearnApiService>();

                string scheme = Request.UrlReferrer.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
                string authority = Request.UrlReferrer.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
                var hostUrl = authority.Replace(scheme, string.Empty);

                var user = d2lService.GetApiObjects<WhoAmIUser>(Request.Url, hostUrl, String.Format(Desire2LearnApiService.WhoAmIUrlFormat, Desire2LearnApiService.ApiVersion));
                var userInfo = d2lService.GetApiObjects<UserData>(Request.Url, hostUrl, String.Format(Desire2LearnApiService.GetUserUrlFormat, Desire2LearnApiService.ApiVersion, user.Identifier));
                string userId = Request.QueryString["x_a"];
                string userKey = Request.QueryString["x_b"];
                string token = null;
                if (!userId.Contains(' ') && !userKey.Contains(' '))
                {
                    token = userId + " " + userKey;
                }

                return AuthCallbackSave(providerKey, token, user.Identifier, userInfo.UserName, "Error");
            }
            else
            {
                try
                {
                    AuthenticationResult result = OAuthWebSecurityWrapper.VerifyAuthentication(provider, this.Settings);
                    if (result.IsSuccessful)
                    {
                        return AuthCallbackSave(providerKey,
                            result.ExtraData.ContainsKey("accesstoken")
                                ? result.ExtraData["accesstoken"]
                                : null,
                            result.ExtraData["id"], null, "Error");
                    }
                    else
                    {
                        var sid = Request.QueryString["__sid__"] ?? string.Empty;
                        var cookie = Request.Cookies[sid];

                        this.ViewBag.Error = string.Format(
                            "Generic OAuth fail: code:[{0}] provider:[{1}] sid:[{2}] cookie:[{3}]",
                            Request.QueryString["code"] ?? string.Empty,
                            Request.QueryString["__provider__"] ?? string.Empty,
                            sid,
                            cookie != null ? cookie.Value : string.Empty);
                    }
                }
                catch (ApplicationException ex)
                {
                    this.ViewBag.Error = string.Format(ex.ToString());
                }
            }

            return this.View("Error");
        }

        /// <summary>
        /// The save settings.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult SaveSettings(LmsUserSettingsDTO settings)
        {
            var lmsProviderName = settings.lmsProviderName;
            var session = this.GetSession(lmsProviderName);
            var companyLms = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
            if (lmsUser == null)
            {
                lmsUser = session.LmsUser ?? new LmsUser { CompanyLms = companyLms, UserId = param.lms_user_id, Username = this.GetUserNameOrEmail(param) };
            }

            lmsUser.AcConnectionMode = (AcConnectionMode)settings.acConnectionMode;
            lmsUser.PrimaryColor = settings.primaryColor;

            if (lmsUser.AcConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
            {
                this.SetACPassword(lmsProviderName, param, settings.password);
            }
            else
            {
                this.RemoveACPassword(param, session);
            }

            this.lmsUserModel.RegisterSave(lmsUser);
            return this.Json(settings);
        }

        /// <summary>
        /// The check password before join.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult CheckPasswordBeforeJoin(string lmsProviderName)
        {
            bool isValid = false;
            var session = this.GetSession(lmsProviderName);
            var companyLms = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
            if (lmsUser != null)
            {
                var mode = lmsUser.AcConnectionMode;
                switch (mode)
                {
                    case AcConnectionMode.DontOverwriteLocalPassword:
                        isValid = !string.IsNullOrWhiteSpace(session.With(x => x.LtiSession).With(x => x.RestoredACPassword));
                        break;
                    default:
                        isValid = true;
                        break;
                }
            }

            return this.Json(new { isValid });
        }

        /// <summary>
        /// The get settings.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public virtual JsonResult GetSettings(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var companyLms = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var lmsUser = this.GetUser(companyLms, param);
            var password = session.With(x => x.LtiSession).With(x => x.RestoredACPassword);
            return
                this.Json(
                    new LmsUserSettingsDTO
                        {
                            acConnectionMode = lmsUser.Item1,
                            primaryColor = lmsUser.Item2,
                            lmsProviderName = lmsProviderName,
                            password = string.IsNullOrWhiteSpace(password) ? null : password
                        });
        }

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
        public virtual JsonResult DeleteRecording(string lmsProviderName, string scoId, string id)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            bool res = this.MeetingSetup.RemoveRecording(
                credentials, 
                this.GetAdobeConnectProvider(credentials), 
                param.course_id, 
                id,
                scoId);
            return this.Json(new { removed = res });
        }

        /// <summary>
        /// The get html page.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult GetHtmlPage(string path)
        {
            return new FilePathResult(path, "text/html");
        }

        /// <summary>
        /// The get meetings.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult GetMeetings(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var meetings = this.MeetingSetup.GetMeetings(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param);

            return this.Json(meetings);
        }

        /// <summary>
        /// The delete meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult DeleteMeeting(string lmsProviderName, string scoId)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var success = this.MeetingSetup.DeleteMeeting(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param,
                scoId);

            return this.Json(success);
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
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            List<RecordingDTO> recordings = this.MeetingSetup.GetRecordings(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param.course_id,
                scoId);

            return this.Json(recordings);
        }

        /// <summary>
        /// The get templates.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetTemplates(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            List<TemplateDTO> templates = this.MeetingSetup.GetTemplates(
                this.GetAdobeConnectProvider(credentials), 
                credentials.ACTemplateScoId);

            return this.Json(templates);
        }

        /// <summary>
        /// The get users.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="forceUpdate">
        /// The force Update.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult GetUsers(string lmsProviderName, string scoId, bool forceUpdate = false)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            string error;
            List<LmsUserDTO> users = this.usersSetup.GetUsers(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param, 
                scoId,
                out error,
                forceUpdate);
            if (string.IsNullOrWhiteSpace(error))
            {
                return this.Json(users);
            }

            return this.Content(error);
        }

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
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var report = this.MeetingSetup.GetAttendanceReport(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param, 
                scoId,
                startIndex, 
                limit);

            return this.Json(report, this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
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
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var report = this.MeetingSetup.GetSessionsReport(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param,
                scoId,
                startIndex,
                limit);

            return this.Json(report, this.IsDebug ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ViewResult"/>.
        /// </returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult Index(LtiParamDTO model)
        {
            string providerName = model.tool_consumer_info_product_family_code;
            return this.LoginWithProvider(providerName, model);
        }

        /// <summary>
        /// The join meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult JoinMeeting(string lmsProviderName, string scoId)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
            string breezeSession = null;
            var url = this.MeetingSetup.JoinMeeting(credentials, param, userSettings, scoId, ref breezeSession, this.GetAdobeConnectProvider(credentials));
            if (url is string)
            {
                this.ViewBag.MeetingUrl = url as string;
                this.ViewBag.BreezeSession = breezeSession;
                this.ViewBag.AcServer = credentials.AcServer.EndsWith("/")
                                            ? credentials.AcServer
                                            : credentials.AcServer + "/";
                return this.View("LoginToAC");
            }

            return this.Json(url, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The leave meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult LeaveMeeting(string lmsProviderName, string scoId)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var result = this.MeetingSetup.LeaveMeeting(credentials, param, scoId, this.GetAdobeConnectProvider(credentials));

            return this.Json(result);
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
        public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
            var url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl);

            if (url is string)
            {
                return this.Redirect(url as string);
            }

            return this.Json(url, JsonRequestBehavior.AllowGet);
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
        public virtual string ShareRecording(string lmsProviderName, string recordingId, bool isPublic, string password)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var link = this.MeetingSetup.UpdateRecording(credentials, this.GetAdobeConnectProvider(credentials), recordingId, isPublic, password);
            
            return link;
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
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
            var url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, "edit");

            if (url is string)
            {
                return this.Redirect(url as string);
            }

            return this.Json(url, JsonRequestBehavior.AllowGet);
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
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
            var url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, "offline");

            if (url is string)
            {
                return this.Redirect(url as string);
            }

            return this.Json(url, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The login with provider.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS Domain.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("register-proxy-tool")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [HttpGet]
        public virtual ActionResult RegisterProxyTool(string lmsDomain)
        {
            if (string.IsNullOrWhiteSpace(lmsDomain))
            {
                this.ViewBag.Error = "Blackboard LMS domain is missing";
                return this.View("Error");
            }

            lmsDomain = lmsDomain.TrimEnd(@"\/".ToCharArray());
            var blackBoardProfile = this.ParseBlackBoardSharedInfo(lmsDomain);
            return this.View(
                "ProxyToolPassword",
                new ProxyToolPasswordModel
                    {
                        LmsDomain = lmsDomain,
                        BlackBoardTitle =
                            string.IsNullOrWhiteSpace(blackBoardProfile.Name)
                                ? lmsDomain
                                : blackBoardProfile.Name,
                        LtiVersion = string.IsNullOrWhiteSpace(blackBoardProfile.LtiVersion) ? "2.0-July08" : blackBoardProfile.LtiVersion
                    });
        }

        /// <summary>
        /// The register proxy tools.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("register-proxy-tool")]
        [HttpPost]
        public virtual ActionResult RegisterProxyTool(ProxyToolPasswordModel model)
        {
            string error;
            if (!this.TryRegisterEGCTool(model, out error))
            {
                this.ViewBag.Error = error;
                return this.View("Error");
            }

            return this.View("ProxyToolRegistrationSucceeded", model);
        }

        /// <summary>
        /// The login with provider.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("login")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult LoginWithProvider(string provider, LtiParamDTO param)
        {
            string lmsProvider = param.GetLtiProviderName(provider);
            if (lmsProvider.ToLower() == LmsProviderNames.Desire2Learn && !String.IsNullOrEmpty(param.user_id))
            {
                var parsedIdArray = param.user_id.Split('_');
                if (parsedIdArray.Length == 2)
                {
                    param.user_id = parsedIdArray[1];
                }
            }
            CompanyLms companyLms = this.CompanyLmsModel.GetOneByProviderAndConsumerKey(lmsProvider, param.oauth_consumer_key).Value;
            if (companyLms != null)
            {
                if (!string.IsNullOrWhiteSpace(companyLms.LmsDomain) && !string.Equals(companyLms.LmsDomain.TrimEnd("/".ToCharArray()), param.lms_domain, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.ViewBag.Error = "This LTI integration is already set for different domain";
                    return this.View("Error");
                }
            }
            else 
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up.");
                return this.View("Error");
            }

            var adobeConnectProvider = this.GetAdobeConnectProvider(companyLms);
            this.SetAdobeConnectProvider(companyLms.Id, adobeConnectProvider);
            
            string email, login;
            this.usersSetup.GetParamLoginAndEmail(param, companyLms, adobeConnectProvider, out email, out login);
            param.ext_user_username = login;
            var lmsUser = this.lmsUserModel.GetOneByUserIdOrUserNameOrEmailAndCompanyLms(param.lms_user_id, param.lms_user_login, param.lis_person_contact_email_primary, companyLms.Id);
            
            var session = this.SaveSession(companyLms, param, lmsUser);
            var key = session.Id.ToString();
            
            this.MeetingSetup.SetupFolders(companyLms, adobeConnectProvider);
            
            if (BltiProviderHelper.VerifyBltiRequest(companyLms, () => this.ValidateLMSDomainAndSaveIfNeeded(param, companyLms)) || this.IsDebug)
            {
                Principal acPrincipal = null;

                switch (lmsProvider.ToLower())
                {
                    case LmsProviderNames.Canvas:
                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token) || CanvasAPI.IsTokenExpired(companyLms.LmsDomain, lmsUser.Token))
                        {
                            this.StartOAuth2Authentication(provider, key, param);
                            return null;
                        }

                        acPrincipal = this.usersSetup.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.lis_person_name_given,
                            param.lis_person_name_family,
                            companyLms.ACUsesEmailAsLogin.GetValueOrDefault());
                        break;
                    case LmsProviderNames.Desire2Learn:
                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                        {
                            var d2lService = IoC.Resolve<IDesire2LearnApiService>();
                            string returnUrl = this.Url.AbsoluteAction(
                                "callback",
                                "Lti",
                                new {__provider__ = provider, providerKey = key},
                                Request.Url.Scheme);

                            return Redirect(d2lService.GetTokenRedirectUrl(new Uri(returnUrl), param.lms_domain).AbsoluteUri);
                        }

                        acPrincipal = this.usersSetup.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.lis_person_name_given,
                            param.lis_person_name_family,
                            companyLms.ACUsesEmailAsLogin.GetValueOrDefault());
                        break;
                    case LmsProviderNames.BrainHoney:
                    case LmsProviderNames.Blackboard:
                    case LmsProviderNames.Moodle:
                    case LmsProviderNames.Sakai:
                        acPrincipal = this.usersSetup.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.lis_person_name_given,
                            param.lis_person_name_family,
                            companyLms.ACUsesEmailAsLogin.GetValueOrDefault());
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser
                                          {
                                              CompanyLms = companyLms,
                                              UserId = param.lms_user_id,
                                              Username = this.GetUserNameOrEmail(param),
                                              PrincipalId = acPrincipal != null ? acPrincipal.PrincipalId : null
                                          };
                            this.lmsUserModel.RegisterSave(lmsUser);
                        }

                        break;
                }

                if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
                {
                    lmsUser.PrincipalId = acPrincipal.PrincipalId;
                    this.lmsUserModel.RegisterSave(lmsUser);
                }

                return this.RedirectToExtJs(companyLms, lmsUser, key);
            }

            this.ViewBag.Error = "Invalid LTI request";
            return this.View("Error");
        }

        /// <summary>
        /// The redirect to error page.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult RedirectToErrorPage(string errorText)
        {
            this.ViewBag["Error"] = errorText;
            return this.View("Error");
        }

        /// <summary>
        /// The update meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult UpdateMeeting(string lmsProviderName, MeetingDTO meeting)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var ret = this.MeetingSetup.SaveMeeting(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param, 
                meeting);

            return this.Json(ret);
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="scoId">
        /// The SCO id
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult UpdateUser(string lmsProviderName, LmsUserDTO user, string scoId)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            string error;
            List<LmsUserDTO> users = this.usersSetup.UpdateUser(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param, 
                user,
                scoId,
                out error);

            if (string.IsNullOrEmpty(error))
            {
                return this.Json(users);
            }

            return this.Content(error);
        }

        /// <summary>
        /// The update user.
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
        public virtual JsonResult SetDefaultRolesForNonParticipants(string lmsProviderName, string scoId)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            List<LmsUserDTO> updatedUser = this.usersSetup.SetDefaultRolesForNonParticipants(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param,
                scoId,
                false);

            return this.Json(updatedUser);
        }

        /// <summary>
        /// The get authentication parameters.
        /// </summary>
        /// <param name="acId">
        /// The AC id.
        /// </param>
        /// <param name="acDomain">
        /// The AC domain.
        /// </param>
        /// <param name="scoId">
        /// The SCO id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public virtual JsonResult GetAuthenticationParameters(string acId, string acDomain, string scoId)
        {
            string error = null;
            var param = this.MeetingSetup.GetLmsParameters(acId, acDomain, scoId, ref error);
            if (param != null)
            {
                return this.Json(param, JsonRequestBehavior.AllowGet);
            }
            
            return this.Json(new { error }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The fix extra data issue.
        /// </summary>
        /// <param name="keyToFix">
        /// The key to fix.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FixExtraDataIssue(string keyToFix)
        {
            if (keyToFix != null && keyToFix.Contains(","))
            {
                var keys = keyToFix.Split(",".ToCharArray());
                keyToFix = keys.FirstOrDefault().Return(x => x, keyToFix);
            }

            return keyToFix;
        }

        /// <summary>
        /// The try register EGC tool.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryRegisterEGCTool(ProxyToolPasswordModel model, out string error)
        {
            var pass = (string)this.Settings.InitialBBPassword;
            var soapApi = IoC.Resolve<SoapAPI>();
            return soapApi.TryRegisterEGCTool(model.LmsDomain, model.RegistrationPassword, pass, out error);
        }

        /// <summary>
        /// The parse black board shared info.
        /// </summary>
        /// <param name="lmsDomain">
        /// The LMS domain.
        /// </param>
        /// <returns>
        /// The <see cref="BBConsumerProfileDTO"/>.
        /// </returns>
        private BBConsumerProfileDTO ParseBlackBoardSharedInfo(string lmsDomain)
        {
            var res = new BBConsumerProfileDTO();
            try
            {
                var uriBuilder = new UriBuilder(lmsDomain + "/webapps/ws/wsadmin/tcprofile");
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var xmlResponse = new WebClient().DownloadString(uriBuilder.Uri);
                var response = XElement.Parse(xmlResponse);
                var name = response.XPathEvaluate("string(/tool-consumer-info/name)").ToString();
                res.Name = name;
                var ltiVersion = response.XPathEvaluate("string(/@ltiVersion)").ToString();
                res.LtiVersion = ltiVersion;
                IEnumerable<XElement> services = response.XPathSelectElements("/services-offered/service");
                var servicesList = services.Select(service => service.XPathEvaluate("string(@name)").ToString()).ToList();
                res.Services = servicesList;
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return res;
        }

        /// <summary>
        /// The get user name or email.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetUserNameOrEmail(LtiParamDTO param)
        {
            return string.IsNullOrWhiteSpace(param.lms_user_login) ? param.lis_person_contact_email_primary : param.lms_user_login;
        }

        /// <summary>
        /// The get user AC connection mode.
        /// </summary>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private Tuple<int, string> GetUser(CompanyLms companyLms, LtiParamDTO param)
        {
            int connectionMode = 0;
            string primaryColor = null;
            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
            if (lmsUser != null)
            {
                connectionMode = (int)lmsUser.AcConnectionMode;
                primaryColor = lmsUser.PrimaryColor;
            }

            return new Tuple<int, string>(connectionMode, primaryColor);
        }

        /// <summary>
        /// The get LMS user settings for join.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserSettingsDTO"/>.
        /// </returns>
        private LmsUserSettingsDTO GetLmsUserSettingsForJoin(string lmsProviderName, CompanyLms companyLms, LtiParamDTO param, LmsUserSession session)
        {
            var lmsUser = this.GetUser(companyLms, param);
            return new LmsUserSettingsDTO
            {
                acConnectionMode = lmsUser.Item1,
                primaryColor = lmsUser.Item2,
                lmsProviderName = lmsProviderName,
                password = session.With(x => x.LtiSession).With(x => x.RestoredACPassword)
            };
        }

        /// <summary>
        /// The validate LMS domain and save if needed.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidateLMSDomainAndSaveIfNeeded(LtiParamDTO param, CompanyLms credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.LmsDomain))
            {
                credentials.LmsDomain = param.lms_domain;
                this.CompanyLmsModel.RegisterSave(credentials, true);
                return true;
            }

            return param.lms_domain.ToLower().Replace("www.", string.Empty).Equals(credentials.LmsDomain.Replace("www.", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The start OAUTH2 authentication.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        private void StartOAuth2Authentication(string provider, string providerKey, LtiParamDTO model)
        {
            string returnUrl = this.Url.AbsoluteAction(
                        "callback",
                        "Lti",
                        new { __provider__ = provider, providerKey },
                        Request.Url.Scheme);
            switch (provider)
            {
                case LmsProviderNames.Canvas:
                    returnUrl = UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Constants.ReturnUriExtensionQueryParameterName, "https://" + model.lms_domain);

                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, providerKey);
                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;
                case LmsProviderNames.Desire2Learn:
                    UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Constants.ReturnUriExtensionQueryParameterName, "https://" + model.lms_domain);

                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;

            }
        }

        private ActionResult AuthCallbackSave(string providerKey, string token, string userId, string username, string viewName)
        {
            LmsUser lmsUser = null;
            var session = this.GetSession(providerKey);
            var company = session.With(x => x.CompanyLms);
            if (!string.IsNullOrEmpty(token))
            {
                var param = session.With(x => x.LtiSession).With(x => x.LtiParam);
                var userName = username ?? this.GetUserNameOrEmail(param);
                lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(userId, company.Id).Value ?? new LmsUser { UserId = userId, CompanyLms = company, Username = userName };
                lmsUser.Username = userName;
                lmsUser.Token = token;
                if (lmsUser.IsTransient())
                {
                    this.SaveSessionUser(session, lmsUser);
                }

                this.lmsUserModel.RegisterSave(lmsUser);
            }

            if (company != null)
            {
                if (company.AdminUser == null && this.IsAdminRole(providerKey))
                {
                    company.AdminUser = lmsUser;
                    CompanyLmsModel.RegisterSave(company);
                }

                return this.RedirectToExtJs(company, lmsUser, providerKey);
            }

            this.ViewBag.Error = string.Format("Credentials not found");
            return View(viewName);
        }

        /// <summary>
        /// The redirect to EXT JS.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        private ActionResult RedirectToExtJs(CompanyLms credentials, LmsUser user, string providerName)
        {
            var primaryColor = user.With(x => x.PrimaryColor);
            this.ViewBag.RedirectUrl = string.Format(
                (this.Settings.LtiPath ?? string.Empty) +
                "/extjs/index.html?primaryColor={0}&lmsProviderName={1}",
                !string.IsNullOrWhiteSpace(primaryColor) ? primaryColor : (credentials.PrimaryColor ?? string.Empty),
                providerName);
            return this.View("Redirect");
        }

        /// <summary>
        /// The is admin role.
        /// </summary>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsAdminRole(string providerName)
        {
            var session = this.GetSession(providerName);
            var param = session.LtiSession.With(x => x.LtiParam);

            if (param == null)
            {
                return this.IsDebug;
            }

            return param.roles.Contains("Administrator");
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLms"/>.
        /// </returns>
        private LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetOneById(uid).Value : null;

            if (this.IsDebug && session == null)
            {
                session = this.userSessionModel.GetOneById(Guid.Empty).Value;
            }

            if (session == null)
            {
                this.RedirectToError("Session timed out. Please refresh the page.");
                return null;
            }

            return session;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        private void RemoveACPassword(LtiParamDTO param, LmsUserSession session)
        {
            if (!string.IsNullOrWhiteSpace(session.With(x => x.LtiSession).With(x => x.RestoredACPassword)))
            {
                var sessionData = new LtiSessionDTO
                {
                    LtiParam = param,
                    ACPasswordData = null,
                    SharedKey = null
                };
                session.SessionData = JsonConvert.SerializeObject(sessionData);
                this.userSessionModel.RegisterSave(session);
            }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="lmsUserSession">
        /// The LMS User Session.
        /// </param>
        /// <returns>
        /// The <see cref="AdobeConnectProvider"/>.
        /// </returns>
        private AdobeConnectProvider GetAdobeConnectProvider(CompanyLms companyLms, LmsUserSession lmsUserSession = null)
        {
            companyLms = companyLms ?? lmsUserSession.Return(x => x.CompanyLms, null);
            AdobeConnectProvider provider = null;
            if (companyLms != null)
            {
                provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, companyLms.Id)] as AdobeConnectProvider;
                if (provider == null)
                {
                    provider = this.MeetingSetup.GetProvider(companyLms);
                    this.SetAdobeConnectProvider(companyLms.Id, provider);
                }
            }

            return provider;
        }

        /// <summary>
        /// The redirect to error.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
        private void RedirectToError(string errorText)
        {
            this.Response.Clear();
            this.Response.Write(string.Format("{{ \"error\": \"{0}\" }}", errorText));
            this.Response.End();
        }

        /// <summary>
        ///     The regenerate id.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void RegenerateId()
        {
            HttpContext httpContext = System.Web.HttpContext.Current;
            var manager = new SessionIDManager();
            string oldId = manager.GetSessionID(httpContext);
            string newId = manager.CreateSessionID(httpContext);

            bool isAdd, isRedirected;
            manager.SaveSessionID(httpContext, newId, out isRedirected, out isAdd);
            HttpApplication application = httpContext.ApplicationInstance;
            HttpModuleCollection modules = application.Modules;
            var ssm = (SessionStateModule)modules.Get("Session");
            FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            SessionStateStoreProviderBase store = null;
            FieldInfo requiredIdField = null, requiredLockIdField = null, requiredStateNotFoundField = null;
            foreach (FieldInfo field in fields)
            {
                if (field.Name.Equals("_store"))
                {
                    store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                }

                if (field.Name.Equals("_rqId"))
                {
                    requiredIdField = field;
                }

                if (field.Name.Equals("_rqLockId"))
                {
                    requiredLockIdField = field;
                }

                if (field.Name.Equals("_rqSessionStateNotFound"))
                {
                    requiredStateNotFoundField = field;
                }
            }

            if (requiredLockIdField != null)
            {
                object lockId = requiredLockIdField.GetValue(ssm);
                if (lockId != null && oldId != null && store != null)
                {
                    store.ReleaseItemExclusive(httpContext, oldId, lockId);
                }
            }

            if (requiredStateNotFoundField != null)
            {
                requiredStateNotFoundField.SetValue(ssm, true);
            }

            if (requiredIdField != null)
            {
                requiredIdField.SetValue(ssm, newId);
            }
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="company">
        /// The credentials.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS User.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="LmsUserSession"/>.
        /// </returns>
        private LmsUserSession SaveSession(CompanyLms company, LtiParamDTO param, LmsUser lmsUser = null, string key = null)
        {
            Guid uid;
            var session = !string.IsNullOrWhiteSpace(key) && Guid.TryParse(key, out uid)
                              ? this.userSessionModel.GetOneById(uid).Value
                              : (lmsUser == null ? null : this.userSessionModel.GetOneByCompanyAndUserAndCourse(company.Id, lmsUser.Id, param.course_id).Value);
            session = session ?? new LmsUserSession { CompanyLms = company, LmsUser = lmsUser, LmsCourseId = param.course_id };
            var sessionData = new LtiSessionDTO { LtiParam = param };
            session.SessionData = JsonConvert.SerializeObject(sessionData);
            this.userSessionModel.RegisterSave(session);

            return session;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="lmsUser">
        /// The LMS User.
        /// </param>
        private void SaveSessionUser(LmsUserSession session, LmsUser lmsUser)
        {
            if (session != null && lmsUser != null)
            {
                session.LmsUser = lmsUser;
                this.userSessionModel.RegisterSave(session);
            }
        }

        /// <summary>
        /// Set AC password
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <param name="adobeConnectPassword">
        /// Adobe Connect password
        /// </param>
        private void SetACPassword(string key, LtiParamDTO param, string adobeConnectPassword)
        {
            if (!string.IsNullOrWhiteSpace(adobeConnectPassword))
            {
                var session = this.GetSession(key);
                var sharedKey = AESGCM.NewKey();
                var sessionData = new LtiSessionDTO
                                      {
                                          LtiParam = param,
                                          ACPasswordData = AESGCM.SimpleEncrypt(adobeConnectPassword, sharedKey),
                                          SharedKey = Convert.ToBase64String(sharedKey)
                                      };
                session.SessionData = JsonConvert.SerializeObject(sessionData);
                this.userSessionModel.RegisterSave(session);
            }
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="acp">
        /// The ACP.
        /// </param>
        private void SetAdobeConnectProvider(int key, AdobeConnectProvider acp)
        {
            this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, key)] = acp;
        }

        #endregion
    }
}