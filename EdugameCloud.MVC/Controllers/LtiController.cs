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
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.Social.OAuth.BrainHoney;
    using EdugameCloud.MVC.Social.OAuth.Canvas;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Providers;

    using Microsoft.Web.WebPages.OAuth;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public partial class LtiController : BaseController
    {
        #region Constants

        /// <summary>
        ///     The credentials session key pattern.
        /// </summary>
        private const string CredentialsSessionKeyPattern = "{0}Credentials";

        /// <summary>
        ///     The parameter session key pattern.
        /// </summary>
        private const string ParamSessionKeyPattern = "{0}Param";

        /// <summary>
        ///     The provider session key pattern.
        /// </summary>
        private const string ProviderSessionKeyPattern = "{0}Provider";

        #endregion

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
            string state = null)
        {
            string provider = __provider__;
            try
            {
                AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication();
                if (result.IsSuccessful)
                {
                    // name of the provider we just used
                    provider = provider ?? result.Provider;
                   
                    if (result.ExtraData.ContainsKey("accesstoken"))
                    {
                        var token = result.ExtraData["accesstoken"];
                        var userId = int.Parse(result.ExtraData["id"]);
                        var userName = result.ExtraData["name"];
                        var company = this.GetCredentials(provider);

                        var lmsUser = this.LmsUserModel.GetOneByUserIdAndCompanyLms(userId, company.Id).Value
                                      ?? new LmsUser { UserId = userId, CompanyLms = company };

                        lmsUser.Username = userName;
                        lmsUser.Token = token;
                        this.lmsUserModel.RegisterSave(lmsUser);
                    }

                    var credentials = this.GetCredentials(provider);

                    if (credentials != null)
                    {
                        this.ViewBag.RedirectUrl = string.Format(
                            "/extjs/index.html?layout={0}&primaryColor={1}&lmsProviderName={2}",
                            credentials.Layout ?? string.Empty,
                            credentials.PrimaryColor ?? string.Empty,
                            provider);
                        return this.View("Redirect");
                    }

                    this.ViewBag.Error = string.Format("Credentials not found");
                }

                this.ViewBag.Error = string.Format("Generic OAuth2 fail");
            }
            catch (ApplicationException ex)
            {
                this.ViewBag.Error = string.Format(ex.ToString());
            }

            return this.View("Error");
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
            string url = this.MeetingSetup.JoinMeeting(credentials, param);

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
            string url = this.MeetingSetup.JoinRecording(credentials, param, recordingUrl);

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
            string providerName = string.IsNullOrWhiteSpace(model.tool_consumer_info_product_family_code)
                                      ? provider
                                      : model.tool_consumer_info_product_family_code.ToLower();
            //// todo: we need to check keys and secrets here var result = ;

            CompanyLms credentials = this.CompanyLmsModel.GetOneByProviderAndDomainOrConsumerKey(
                    providerName, 
                    model.lms_domain, 
                    model.oauth_consumer_key).Value;
            if (credentials != null)
            {
                this.SetParam(providerName, model);
                this.SetCredentials(providerName, credentials);

                this.MeetingSetup.SetupFolders(this.GetCredentials(providerName), this.GetProvider(providerName));
            }
            else if (!this.IsDebug)
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up. Please go to <a href=\"{0}\">{0}</a> to set it.", this.Settings.EGCUrl);
                return this.View("Error");
            }
            else
            {
                credentials = this.GetCredentials(providerName);
                this.SetDebugModelValues(model, providerName);
            }

            if (credentials.AdminUser == null)
            {
                this.ViewBag.Error = "We don't have admin user for these settings. Please do OAuth.";
                return this.View("Error");
            }

            this.AddSessionCookie(this.Session.SessionID);

            switch (providerName.ToLower())
            {
                case LmsProviderNames.Canvas:
                    if (BltiProviderHelper.VerifyBltiRequest(
                        credentials,
                        () => this.ValidateLMSDomainAndSaveIfNeeded(model, credentials)) || this.IsDebug)
                    {
                        var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(model.lms_user_id, credentials.Id).Value;
                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                        {
                            this.StartOAuth2Authentication(provider, model);
                            return null;
                        }

                        return this.RedirectToExtJs(credentials, providerName);
                    }

                    break;
                case LmsProviderNames.BrainHoney:
                    if (BltiProviderHelper.VerifyBltiRequest(credentials, () => this.ValidateLMSDomainAndSaveIfNeeded(model, credentials)) || this.IsDebug)
                    {
                        return this.RedirectToExtJs(credentials, providerName);
                    }

                    break;
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

        #endregion

        #region Methods

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
                    break;
                case LmsProviderNames.BrainHoney:
                    model.tool_consumer_instance_guid = "pacybersandbox-connect.brainhoney.com";
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
        /// <param name="model">
        /// The model.
        /// </param>
        private void StartOAuth2Authentication(string provider, LtiParamDTO model)
        {
            string returnUrl = this.Url.AbsoluteAction(EdugameCloudT4.Lti.AuthenticationCallback(provider));
            returnUrl = CanvasClient.AddCanvasUrlToReturnUrl(returnUrl, "https://" + model.lms_domain);
            OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
        }

        /// <summary>
        /// The redirect to EXT JS.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        private ActionResult RedirectToExtJs(CompanyLms credentials, string providerName)
        {
            this.ViewBag.RedirectUrl = string.Format(
                "/extjs/index.html?layout={0}&primaryColor={1}&lmsProviderName={2}",
                credentials.Layout ?? string.Empty,
                credentials.PrimaryColor ?? string.Empty,
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
            this.Response.Cookies.Add(
                new HttpCookie(this.Settings.SessionCookieName, newId)
                    {
                        Domain = this.Settings.CookieDomain, 
                        Secure = true, 
                        Path = this.Settings.CookiePath
                    });
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
            var creds = this.Session[string.Format(CredentialsSessionKeyPattern, providerName)] as CompanyLms;

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
                }
            }

            if (creds == null)
            {
                this.RedirectToError("Session timed out. Please refresh the page");
                return null;
            }

            return creds;
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
            var model = this.Session[string.Format(ParamSessionKeyPattern, providerName)] as LtiParamDTO;

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
                                        custom_canvas_user_id = 3969969, 
                                        tool_consumer_info_product_family_code = "canvas", 
                                        custom_canvas_api_domain = "canvas.instructure.com"
                                    };
                        break;
                    case LmsProviderNames.BrainHoney:
                        model = new LtiParamDTO
                                    {
                                        context_id = "24955426",
                                        user_id = 24955385, 
                                        lis_person_contact_email_primary = "mike@esynctraining.com", 
                                        roles = "Administrator", 
                                        tool_consumer_info_product_family_code = "BrainHoney", 
                                        tool_consumer_instance_guid = "pacybersandbox-connect.brainhoney.com"
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
            var provider = this.Session[string.Format(ProviderSessionKeyPattern, providerName)] as AdobeConnectProvider;

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
            this.Session[string.Format(CredentialsSessionKeyPattern, providerName)] = credentials;
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
            this.Session[string.Format(ParamSessionKeyPattern, providerName)] = param;
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
            this.Session[string.Format(ProviderSessionKeyPattern, providerName)] = acp;
        }

        #endregion
    }
}