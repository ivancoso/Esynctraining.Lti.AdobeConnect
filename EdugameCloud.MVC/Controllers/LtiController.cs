﻿namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    using DotNetOpenAuth.AspNet;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.Lti.Utils;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.Social.OAuth;
    using EdugameCloud.MVC.Social.OAuth.Canvas;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Microsoft.Web.WebPages.OAuth;

    using Newtonsoft.Json;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public partial class LtiController : BaseController
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
        public LtiController(CompanyLmsModel companyLmsModel, LmsUserSessionModel userSessionModel, LmsUserModel lmsUserModel, MeetingSetup meetingSetup, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.companyLmsModel = companyLmsModel;
            this.userSessionModel = userSessionModel;
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
        }

        #endregion

        #region Properties

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
            string provider = __provider__;

            try
            {
                AuthenticationResult result = OAuthWebSecurityWrapper.VerifyAuthentication(provider, this.Settings);
                if (result.IsSuccessful)
                {
                    LmsUser lmsUser = null;
                    var session = this.GetSession(providerKey);
                    var company = session.With(x => x.CompanyLms);
                    if (result.ExtraData.ContainsKey("accesstoken"))
                    {
                        var token = result.ExtraData["accesstoken"];
                        var userId = result.ExtraData["id"];
                        var param = session.With(x => x.LtiSession).With(x => x.LtiParam);
                        var userName = this.GetUserNameOrEmail(param);
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
                lmsUser = new LmsUser { CompanyLms = companyLms, UserId = param.lms_user_id, Username = this.GetUserNameOrEmail(param) };
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
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult DeleteRecording(string lmsProviderName, string id)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            bool res = this.MeetingSetup.RemoveRecording(
                credentials, 
                this.GetAdobeConnectProvider(credentials), 
                param.course_id, 
                id);
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
        /// The get meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetMeeting(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            MeetingDTO meeting = this.MeetingSetup.GetMeeting(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param);

            return this.Json(meeting);
        }

        /// <summary>
        /// The delete meeting.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The lms provider name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult DeleteMeeting(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var success = this.MeetingSetup.DeleteMeeting(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param);

            return this.Json(success);
        }

        /// <summary>
        /// The get recordings.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            List<RecordingDTO> recordings = this.MeetingSetup.GetRecordings(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param.course_id);

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
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult GetUsers(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            string error;
            List<LmsUserDTO> users = this.MeetingSetup.GetUsers(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param, 
                out error);
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
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult GetAttendanceReport(string lmsProviderName, int startIndex = 0, int limit = 0)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var report = this.MeetingSetup.GetAttendanceReport(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param, 
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
        /// <param name="startIndex">
        /// The start Index.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult GetSessionsReport(string lmsProviderName, int startIndex = 0, int limit = 0)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var report = this.MeetingSetup.GetSessionsReport(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param,
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
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult JoinMeeting(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
            string url = this.MeetingSetup.JoinMeeting(credentials, param, userSettings, this.GetAdobeConnectProvider(credentials));

            return this.Redirect(url);
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
            string url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl);

            if (url == null)
            {
                this.RedirectToError("Can not access the recording");
            }

            return this.Redirect(url);
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
            string url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, "edit");

            if (url == null)
            {
                this.RedirectToError("Can not access the recording");
            }

            return this.Redirect(url);
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
            string url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, "offline");

            if (url == null)
            {
                this.RedirectToError("Can not access the recording");
            }

            return this.Redirect(url);
        }

        /// <summary>
        /// The login with provider.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("login")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult LoginWithProvider(string provider, LtiParamDTO model)
        {
            string lmsProvider = model.GetLtiProviderName(provider);

            CompanyLms credentials = this.CompanyLmsModel.GetOneByProviderAndConsumerKey(lmsProvider, model.oauth_consumer_key).Value;
            if (credentials != null)
            {
                if (!string.IsNullOrWhiteSpace(credentials.LmsDomain) && !string.Equals(credentials.LmsDomain.TrimEnd("/".ToCharArray()), model.lms_domain, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.ViewBag.Error = "This LTI integration is already set for different domain";
                    return this.View("Error");
                }
            }
            else 
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up. Please go to <a href=\"{0}\">{0}</a> to set it.", this.Settings.EGCUrl);
                return this.View("Error");
            }

            var lmsUser = this.lmsUserModel.GetOneByUserIdOrUserNameOrEmailAndCompanyLms(model.lms_user_id, model.lms_user_login, model.lis_person_contact_email_primary, credentials.Id);
            var session = this.SaveSession(credentials, model, lmsUser);
            var key = session.Id.ToString();
            var adobeConnectProvider = this.GetAdobeConnectProvider(credentials);
            this.MeetingSetup.SetupFolders(credentials, adobeConnectProvider);
            this.SetAdobeConnectProvider(credentials.Id, adobeConnectProvider);
            
            if (BltiProviderHelper.VerifyBltiRequest(credentials, () => this.ValidateLMSDomainAndSaveIfNeeded(model, credentials)) || this.IsDebug)
            {
                switch (lmsProvider.ToLower())
                {
                    case LmsProviderNames.Canvas:

                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                        {
                            this.StartOAuth2Authentication(provider, key, model);
                            return null;
                        }

                        return this.RedirectToExtJs(credentials, lmsUser, key);

                    case LmsProviderNames.BrainHoney:
                    case LmsProviderNames.Blackboard:
                    case LmsProviderNames.Moodle:
                    case LmsProviderNames.Sakai:
                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser { CompanyLms = credentials, UserId = model.lms_user_id, Username = this.GetUserNameOrEmail(model) };
                            this.lmsUserModel.RegisterSave(lmsUser);
                        }

                        return this.RedirectToExtJs(credentials, lmsUser, key);
                }
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
            MeetingDTO ret = this.MeetingSetup.SaveMeeting(
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
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult UpdateUser(string lmsProviderName, LmsUserDTO user)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            string error;
            List<LmsUserDTO> users = this.MeetingSetup.UpdateUser(
                credentials,
                this.GetAdobeConnectProvider(credentials), 
                param, 
                user,
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
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult SetDefaultRolesForNonParticipants(string lmsProviderName)
        {
            var session = this.GetSession(lmsProviderName);
            var credentials = session.CompanyLms;
            var param = session.LtiSession.With(x => x.LtiParam);
            List<LmsUserDTO> updatedUser = this.MeetingSetup.SetDefaultRolesForNonParticipants(
                credentials,
                this.GetAdobeConnectProvider(credentials),
                param);

            return this.Json(updatedUser);
        }

        #endregion

        #region Methods

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
            string returnUrl = this.Url.AbsoluteAction(EdugameCloudT4.Lti.AuthenticationCallback(provider, null, null, null, providerKey));
            returnUrl = CanvasClient.AddCanvasUrlToReturnUrl(returnUrl, "https://" + model.lms_domain);
            returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, providerKey);
            OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
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