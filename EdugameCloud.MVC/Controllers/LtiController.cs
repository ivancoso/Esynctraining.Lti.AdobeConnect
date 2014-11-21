namespace EdugameCloud.MVC.Controllers
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
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.Social.OAuth;
    using EdugameCloud.MVC.Social.OAuth.Canvas;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Microsoft.Web.WebPages.OAuth;

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
        /// <param name="lmsUserModel">
        /// LMS user model
        /// </param>
        /// <param name="meetingSetup">
        /// The meeting Setup.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public LtiController(CompanyLmsModel companyLmsModel, LmsUserModel lmsUserModel, MeetingSetup meetingSetup, ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.companyLmsModel = companyLmsModel;
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
                    // name of the provider we just used
                    provider = provider ?? result.Provider;

                    LmsUser lmsUser = null;

                    if (result.ExtraData.ContainsKey("accesstoken"))
                    {
                        var token = result.ExtraData["accesstoken"];
                        var userId = result.ExtraData["id"];
                        var userName = result.ExtraData["name"];
                        var company = this.GetCredentials(providerKey);

                        lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(userId, company.Id).Value
                                      ?? new LmsUser { UserId = userId, CompanyLms = company };

                        lmsUser.Username = userName;
                        lmsUser.Token = token;
                        this.lmsUserModel.RegisterSave(lmsUser);
                    }

                    var credentials = this.GetCredentials(providerKey);

                    if (credentials != null)
                    {
                        if (credentials.AdminUser == null && this.IsAdminRole(providerKey))
                        {
                            credentials.AdminUser = lmsUser;
                            CompanyLmsModel.RegisterSave(credentials);
                        }

                        return this.RedirectToExtJs(credentials, lmsUser, providerKey);
                    }

                    this.ViewBag.Error = string.Format("Credentials not found");
                }
                else
                {
                    this.ViewBag.Error = string.Format("Generic OAuth fail: {0}", result.Error != null ? result.Error.Message : (result.Provider ?? string.Empty));
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
            CompanyLms companyLms = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
            if (lmsUser == null)
            {
                lmsUser = new LmsUser { CompanyLms = companyLms, UserId = param.lms_user_id };
            }

            lmsUser.AcConnectionMode = (AcConnectionMode)settings.acConnectionMode;
            lmsUser.PrimaryColor = settings.primaryColor;

            if (lmsUser.AcConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
            {
                this.SetACPassword(companyLms, param, settings.password);
            }
            else
            {
                this.RemoveACPassword(companyLms, param);
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
            CompanyLms companyLms = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
            if (lmsUser != null)
            {
                var mode = lmsUser.AcConnectionMode;
                switch (mode)
                {
                    case AcConnectionMode.DontOverwriteLocalPassword:
                        isValid = !string.IsNullOrWhiteSpace(this.GetACPassword(companyLms, param));
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
            CompanyLms companyLms = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var lmsUser = this.GetUser(companyLms, param);
            var password = this.GetACPassword(companyLms, param);
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            bool res = this.MeetingSetup.RemoveRecording(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            MeetingDTO meeting = this.MeetingSetup.GetMeeting(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
                param);

            return this.Json(meeting);
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            List<RecordingDTO> recordings = this.MeetingSetup.GetRecordings(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            List<TemplateDTO> templates = this.MeetingSetup.GetTemplates(
                this.MeetingSetup.GetProvider(credentials), 
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
        public virtual JsonResult GetUsers(string lmsProviderName)
        {
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            List<LmsUserDTO> users = this.MeetingSetup.GetUsers(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
                param);

            return this.Json(users);
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var report = this.MeetingSetup.GetAttendanceReport(
                credentials,
                this.MeetingSetup.GetProvider(credentials),
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var report = this.MeetingSetup.GetSessionsReport(
                credentials,
                this.MeetingSetup.GetProvider(credentials),
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param);
            string url = this.MeetingSetup.JoinMeeting(credentials, param, userSettings);

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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param);
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
        /// The lms provider name.
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param);
            var link = this.MeetingSetup.UpdateRecording(credentials, this.GetProvider(lmsProviderName), recordingId, isPublic, password);
            
            return link;
        }

        /// <summary>
        /// The edit recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The lms provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param);
            string url = this.MeetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, "edit");

            if (url == null)
            {
                this.RedirectToError("Can not access the recording");
            }

            return this.Redirect(url);
        }

        /// <summary>
        /// The get recording flv.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The lms provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param);
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
            string lmsKey = model.oauth_consumer_key;
            string lmsProvider = this.GetProviderName(provider, model);

            CompanyLms credentials = this.CompanyLmsModel.GetOneByProviderAndConsumerKey(
                    lmsProvider,
                    model.oauth_consumer_key).Value;
            if (credentials != null)
            {
                if (credentials.LmsDomain != null && !string.Equals(credentials.LmsDomain, model.lms_domain, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.ViewBag.Error = "This LTI integration is already set for different domain";
                    return this.View("Error");
                }

                this.SetParam(lmsKey, model);
                this.SetCredentials(lmsKey, credentials);

                this.MeetingSetup.SetupFolders(this.GetCredentials(lmsKey), this.GetProvider(lmsKey));
            }
            else if (!this.IsDebug)
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up. Please go to <a href=\"{0}\">{0}</a> to set it.", this.Settings.EGCUrl);
                return this.View("Error");
            }
            else
            {
                credentials = this.GetCredentials(lmsKey);
                this.SetDebugModelValues(model, lmsProvider);
            }
            
            /*
            if (credentials.AdminUser == null && !this.IsAdminRole(providerName))
            {
                this.ViewBag.Error = "We don't have admin user for these settings. Please do OAuth.";
                return this.View("Error");
            }
            */

            this.AddSessionCookie(this.Session.SessionID);

            var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(model.lms_user_id, credentials.Id).Value;
            
            if (BltiProviderHelper.VerifyBltiRequest(
                credentials,
                () => this.ValidateLMSDomainAndSaveIfNeeded(model, credentials)) || this.IsDebug)
            {
                switch (lmsProvider.ToLower())
                {
                    case LmsProviderNames.Canvas:

                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                        {
                            this.StartOAuth2Authentication(provider, lmsKey, model);
                            return null;
                        }

                        return this.RedirectToExtJs(credentials, lmsUser, lmsKey);

                    case LmsProviderNames.BrainHoney:
                    case LmsProviderNames.Blackboard:
                    case LmsProviderNames.Moodle:
                        return this.RedirectToExtJs(credentials, lmsUser, lmsKey);
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            MeetingDTO ret = this.MeetingSetup.SaveMeeting(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
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
        public virtual JsonResult UpdateUser(string lmsProviderName, LmsUserDTO user)
        {
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            List<LmsUserDTO> updatedUser = this.MeetingSetup.UpdateUser(
                credentials, 
                this.MeetingSetup.GetProvider(credentials), 
                param, 
                user);

            return this.Json(updatedUser);
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
            CompanyLms credentials = this.GetCredentials(lmsProviderName);
            LtiParamDTO param = this.GetParam(lmsProviderName);
            List<LmsUserDTO> updatedUser = this.MeetingSetup.SetDefaultRolesForNonParticipants(
                credentials,
                this.MeetingSetup.GetProvider(credentials),
                param);

            return this.Json(updatedUser);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get provider name.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetProviderName(string provider, LtiParamDTO model)
        {
            var providerName = string.IsNullOrWhiteSpace(model.tool_consumer_info_product_family_code)
                       ? provider
                       : model.tool_consumer_info_product_family_code.ToLower();
            if (provider.Equals(LmsProviderNames.Blackboard, StringComparison.OrdinalIgnoreCase))
            {
                const string PatternToRemove = " learn";
                if (providerName.EndsWith(PatternToRemove, StringComparison.OrdinalIgnoreCase))
                {
                    providerName = providerName.Replace(PatternToRemove, string.Empty);
                }
            }

            return providerName;
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
        /// <returns>
        /// The <see cref="LmsUserSettingsDTO"/>.
        /// </returns>
        private LmsUserSettingsDTO GetLmsUserSettingsForJoin(string lmsProviderName, CompanyLms companyLms, LtiParamDTO param)
        {
            var lmsUser = this.GetUser(companyLms, param);
            return new LmsUserSettingsDTO
            {
                acConnectionMode = lmsUser.Item1,
                primaryColor = lmsUser.Item2,
                lmsProviderName = lmsProviderName,
                password = this.GetACPassword(companyLms, param)
            };
        }

        /// <summary>
        /// The set debug model values.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        private void SetDebugModelValues(LtiParamDTO model, string providerName)
        {
            switch (providerName.ToLower())
            {
                case LmsProviderNames.Canvas:
                    model.custom_canvas_api_domain = "canvas.instructure.com";
                    model.lis_person_contact_email_primary = "mike@esynctraining.com";
                    break;
                case LmsProviderNames.BrainHoney:
                    model.tool_consumer_instance_guid = "pacybersandbox-connect.brainhoney.com";
                    model.lis_person_contact_email_primary = "mike@esynctraining.com";
                    break;
            }
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
        /// The add session cookie.
        /// </summary>
        /// <param name="newId">
        /// The new id.
        /// </param>
        private void AddSessionCookie(string newId)
        {
            this.Response.Cookies.Remove(this.Settings.SessionCookieName);
            var sessionCookie = new HttpCookie(this.Settings.SessionCookieName, newId);

            sessionCookie.Expires = DateTime.Now.AddMinutes(Session.Timeout);

            this.Response.Cookies.Add(sessionCookie);
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLms"/>.
        /// </returns>
        private CompanyLms GetCredentials(string providerName)
        {
            var creds = this.Session[string.Format(LtiSessionKeys.CredentialsSessionKeyPattern, providerName)] as CompanyLms;

            if (creds == null && this.IsDebug)
            {
                switch (providerName.ToLower())
                {
                    case LmsProviderNames.Canvas:
                        creds = this.CompanyLmsModel.GetOneByDomain("canvas.instructure.com").Value;
                        break;
                    case LmsProviderNames.BrainHoney:
                        creds = this.CompanyLmsModel.GetOneByDomain("pacybersandbox-connect.brainhoney.com").Value;
                        break;
                    case LmsProviderNames.Blackboard:
                        creds = this.CompanyLmsModel.GetOneByDomain("blackboard.advantageconnectpro.com").Value;
                        break;
                }
            }

            if (creds == null)
            {
                this.RedirectToError("Session timed out. Please refresh the page.");
                return null;
            }

            return creds;
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
            var param = this.GetParam(providerName);

            if (param == null)
            {
                return this.IsDebug;
            }

            return param.roles.Contains("Administrator");
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="LtiParamDTO"/>.
        /// </returns>
        private LtiParamDTO GetParam(string providerName)
        {
            var model = this.Session[string.Format(LtiSessionKeys.ParamSessionKeyPattern, providerName)] as LtiParamDTO;

            if (model == null && this.IsDebug)
            {
                switch (providerName.ToLower())
                {
                    case LmsProviderNames.Canvas:
                        model = new LtiParamDTO
                                    {
                                        custom_canvas_course_id = 865831, 
                                        lis_person_contact_email_primary = "mike@esynctraining.com", 
                                        roles = "Administrator", 
                                        custom_canvas_user_id = "3969969", 
                                        tool_consumer_info_product_family_code = "canvas", 
                                        custom_canvas_api_domain = "canvas.instructure.com"
                                    };
                        break;
                    case LmsProviderNames.BrainHoney:
                        model = new LtiParamDTO
                                    {
                                        context_id = "24955426",
                                        user_id = "24955385", 
                                        lis_person_contact_email_primary = "mike@esynctraining.com", 
                                        roles = "Administrator", 
                                        tool_consumer_info_product_family_code = "BrainHoney", 
                                        tool_consumer_instance_guid = "pacybersandbox-connect.brainhoney.com"
                                    };
                        break;
                    case LmsProviderNames.Blackboard:
                        model = new LtiParamDTO
                        {
                            context_id = "cf1784b1cada44ddb512e47f66dd4ad7",
                            user_id = "05f3cadd856e45d080a9c70a0bc0e7fb",
                            lis_person_contact_email_primary = "mike@esynctraining.com",
                            roles = "urn:lti:role:ims/lis/Instructor",
                            tool_consumer_info_product_family_code = "Blackboard Learn",
                            tool_consumer_instance_guid = "e4b016ab2cef47f58a40bb42648cb0ab",
                            launch_presentation_return_url = "http://blackboard.advantageconnectpro.com/webapps/blackboard/execute/blti/launchReturn?course_id=_6_1&content_id=_44_1&toGC=false"
                        };
                        break;
                }
            }

            if (model == null)
            {
                this.RedirectToError("You are not logged in.");
                return null;
            }

            return model;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="LtiParamDTO"/>.
        /// </returns>
        private string GetACPassword(CompanyLms company, LtiParamDTO data)
        {
            var key = this.GetACPasswordSessionKey(company, data);
            var model = this.Session[string.Format(LtiSessionKeys.ACPasswordSessionKeyPattern, key)] as string;
            return model;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="company">
        /// The company.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        private void RemoveACPassword(CompanyLms company, LtiParamDTO data)
        {
            if (!string.IsNullOrWhiteSpace(this.GetACPassword(company, data)))
            {
                var key = this.GetACPasswordSessionKey(company, data);
                this.Session[string.Format(LtiSessionKeys.ACPasswordSessionKeyPattern, key)] = null;
            }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <returns>
        /// The <see cref="AdobeConnectProvider"/>.
        /// </returns>
        private AdobeConnectProvider GetProvider(string providerName)
        {
            var provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, providerName)] as AdobeConnectProvider;

            if (provider == null)
            {
                provider = this.MeetingSetup.GetProvider(this.GetCredentials(providerName));
                this.SetProvider(providerName, provider);
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

            this.AddSessionCookie(newId);

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
        /// Sets the credentials.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        private void SetCredentials(string providerName, CompanyLms credentials)
        {
            this.Session[string.Format(LtiSessionKeys.CredentialsSessionKeyPattern, providerName)] = credentials;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <param name="param">
        /// The parameter.
        /// </param>
        private void SetParam(string providerName, LtiParamDTO param)
        {
            this.Session[string.Format(LtiSessionKeys.ParamSessionKeyPattern, providerName)] = param;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="adobeConnectPassword">
        /// Adobe Connect password
        /// </param>
        private void SetACPassword(CompanyLms companyLms, LtiParamDTO data, string adobeConnectPassword)
        {
            var key = this.GetACPasswordSessionKey(companyLms, data);
            this.Session[string.Format(LtiSessionKeys.ACPasswordSessionKeyPattern, key)] = adobeConnectPassword;
        }

        /// <summary>
        /// The get ac password session key.
        /// </summary>
        /// <param name="companyLms">
        /// The company LMS.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetACPasswordSessionKey(CompanyLms companyLms, LtiParamDTO data)
        {
            var key = companyLms.AcServer.ToLowerInvariant()
                      + (string.IsNullOrWhiteSpace(data.lis_person_contact_email_primary)
                             ? data.lms_user_login
                             : data.lis_person_contact_email_primary).Return(x => x.ToLowerInvariant(), string.Empty);
            return key;
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="providerName">
        /// The provider Name.
        /// </param>
        /// <param name="acp">
        /// The ACP.
        /// </param>
        private void SetProvider(string providerName, AdobeConnectProvider acp)
        {
            this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, providerName)] = acp;
        }

        #endregion
    }
}