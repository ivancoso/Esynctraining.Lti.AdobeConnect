namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    using Castle.Core.Logging;

    using DocumentFormat.OpenXml.InkML;

    using DotNetOpenAuth.AspNet;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.MVC.HtmlHelpers;
    using EdugameCloud.MVC.Social.OAuth.Canvas;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Microsoft.Web.WebPages.OAuth;

    using UserDTO = EdugameCloud.Lti.DTO.UserDTO;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public partial class LtiController : BaseController
    {
        #region Static Fields

        /// <summary>
        ///     The is debug.
        /// </summary>
        private static bool? isDebug = false;

        #endregion

        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The RTMP model.
        /// </summary>
        private readonly RTMPModel rtmpModel;

        /// <summary>
        /// The social user tokens model.
        /// </summary>
        private readonly SocialUserTokensModel socialUserTokensModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LtiController"/> class.
        /// </summary>
        /// <param name="socialUserTokensModel">
        /// The social User Tokens Model.
        /// </param>
        /// <param name="rtmpModel">
        /// The rtmp Model.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public LtiController(
            SocialUserTokensModel socialUserTokensModel, 
            RTMPModel rtmpModel, 
            ILogger logger, 
            ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.socialUserTokensModel = socialUserTokensModel;
            this.rtmpModel = rtmpModel;
            this.logger = logger;
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
                return IoC.Resolve<CompanyLmsModel>();
            }
        }

        /// <summary>
        ///     Gets the credentials.
        /// </summary>
        private CompanyLms Credentials
        {
            get
            {
                var creds = this.Session["Credentials"] as CompanyLms;

                if (creds == null && this.IsDebug)
                {
                    creds = this.CompanyLmsModel.GetOneByDomain("canvas.instructure.com").Value;
                }

                if (creds == null)
                {
                    this.RedirectToError("Session timed out. Please refresh the Canvas page and press Join again.");
                    return null;
                }

                return creds;
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
                return IoC.Resolve<MeetingSetup>();
            }
        }

        /// <summary>
        ///     Gets the parameter.
        /// </summary>
        private LtiParamDTO Param
        {
            get
            {
                var model = this.Session["Param"] as LtiParamDTO;

                if (model == null && this.IsDebug)
                {
                    model = new LtiParamDTO
                                {
                                    custom_canvas_course_id = 865831, 
                                    lis_person_contact_email_primary = "mike@esynctraining.com"
                                };
                }

                if (model == null)
                {
                    this.RedirectToError("You are not logged in.");
                    return null;
                }

                return model;
            }
        }

        /// <summary>
        ///     Gets the provider.
        /// </summary>
        private AdobeConnectProvider Provider
        {
            get
            {
                var provider = this.Session["Provider"] as AdobeConnectProvider;

                if (provider == null)
                {
                    provider = this.MeetingSetup.GetProvider(this.Credentials);
                    this.Session["Provider"] = provider;
                }

                return provider;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The authentication callback.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ActionName("callback")]
        [AllowAnonymous]
        public virtual ActionResult AuthenticationCallback(string __provider__, string __sid__ = null, string code = null, string state = null)
        {
            var provider = __provider__;
            string error;
            try
            {
                AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication();
                if (result.IsSuccessful)
                {
                    // name of the provider we just used
                    provider = provider ?? result.Provider;

                    // dictionary of values from identity provider
                    IDictionary<string, string> extra = result.ExtraData;

                    if (extra.ContainsKey("accesstoken"))
                    {
                        string token = extra["accesstoken"];
                        string secret = string.Empty;

//                        if (!string.IsNullOrWhiteSpace(key))
//                        {
//                            SocialUserTokens tokens = this.socialUserTokensModel.GetOneByKey(key).Value;
//                            tokens = tokens ?? new SocialUserTokens();
//                            tokens.Key = key;
//                            tokens.Provider = provider;
//                            tokens.Token = token;
//                            tokens.Secret = secret;
//                            this.socialUserTokensModel.RegisterSave(tokens, true);
//                            try
//                            {
//                                this.rtmpModel.NotifyClientsAboutSocialTokens(new SocialUserTokensDTO(tokens));
//                            }
//                            catch (Exception ex)
//                            {
//                            }
//                        }

                        return this.Content("Login successful. Token:" + token);
                    }
                }

                error = result.Error.Return(x => x.ToString(), "Generic fail");
            }
            catch (ApplicationException ex)
            {
                error = ex.Message;
            }

            return this.Content(error);
        }

        /// <summary>
        /// The delete recording.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public virtual JsonResult DeleteRecording(string id)
        {
            bool res = this.MeetingSetup.RemoveRecording(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param.custom_canvas_course_id, 
                id);
            return this.Json(new { removed = res });
        }

        /// <summary>
        ///     The get meeting.
        /// </summary>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetMeeting()
        {
            MeetingDTO meeting = this.MeetingSetup.GetMeeting(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param);

            return this.Json(meeting);
        }

        /// <summary>
        ///     The get recordings.
        /// </summary>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetRecordings()
        {
            List<RecordingDTO> recordings = this.MeetingSetup.GetRecordings(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param.custom_canvas_course_id);

            return this.Json(recordings);
        }

        /// <summary>
        ///     The get templates.
        /// </summary>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetTemplates()
        {
            List<TemplateDTO> templates = this.MeetingSetup.GetTemplates(
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Credentials.ACTemplateScoId);

            return this.Json(templates);
        }

        /// <summary>
        ///     The get users.
        /// </summary>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetUsers()
        {
            List<UserDTO> users = this.MeetingSetup.GetUsers(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param);

            return this.Json(users);
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ViewResult"/>.
        /// </returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult Index(LtiParamDTO model)
        {
            CompanyLms credentials = this.CompanyLmsModel.GetOneByDomainOrConsumerKey(model.custom_canvas_api_domain, model.oauth_consumer_key).Value;
            if (credentials != null)
            {
                this.Session["Param"] = model;
                this.Session["Credentials"] = credentials;

                this.MeetingSetup.SetupFolders(this.Credentials, this.Provider);
            }
            else if (!this.IsDebug)
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up. Please go to <a href=\"{0}\">{0}</a> to set it.", this.Settings.EGCUrl);
                return this.View("Error");
            }
            else
            {
                credentials = this.Credentials;
            }

            if (credentials.AdminUser == null)
            {
                this.ViewBag.Error = "We don't have admin user for these settings. Please do OAuth.";
                return this.View("Error");
            }

            this.AddSessionCookie(this.Session.SessionID);

            if (credentials != null)
            {
                return this.Redirect(
                    string.Format(
                        "/extjs/index.html?layout={0}&primaryColor={1}",
                        credentials.Layout ?? string.Empty,
                        credentials.PrimaryColor ?? string.Empty));
            }

            return null;
        }

        /// <summary>
        ///     The join meeting.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public virtual ActionResult JoinMeeting()
        {
            string url = this.MeetingSetup.JoinMeeting(this.Credentials, this.Param);

            return this.Redirect(url);
        }

        /// <summary>
        /// The join recording.
        /// </summary>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public virtual ActionResult JoinRecording(string recordingUrl)
        {
            string url = this.MeetingSetup.JoinRecording(this.Credentials, this.Param, recordingUrl);

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
            CompanyLms credentials = this.CompanyLmsModel.GetOneByDomainOrConsumerKey(model.custom_canvas_api_domain, model.oauth_consumer_key).Value;
            if (credentials != null)
            {
                this.Session["Param"] = model;
                this.Session["Credentials"] = credentials;

                this.MeetingSetup.SetupFolders(this.Credentials, this.Provider);
            }
            else if (!this.IsDebug)
            {
                this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up. Please go to <a href=\"{0}\">{0}</a> to set it.", this.Settings.EGCUrl);
                return this.View("Error");
            }
            else
            {
                credentials = this.Credentials;
            }

            if (credentials.AdminUser == null)
            {
                this.ViewBag.Error = "We don't have admin user for these settings. Please do OAuth.";
                return this.View("Error");
            }

            this.AddSessionCookie(this.Session.SessionID);
            string returnUrl = this.Url.AbsoluteAction(EdugameCloudT4.Lti.AuthenticationCallback(provider));
            returnUrl = CanvasClient.AddCanvasUrlToReturnUrl(returnUrl, string.IsNullOrWhiteSpace(model.launch_presentation_return_url) ? "https://" + model.custom_canvas_api_domain : new Uri(model.launch_presentation_return_url).GetLeftPart(UriPartial.Authority));
            OAuthWebSecurity.RequestAuthentication(provider, returnUrl);

            return null;
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
        /// <param name="meeting">
        /// The meeting.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult UpdateMeeting(MeetingDTO meeting)
        {
            MeetingDTO ret = this.MeetingSetup.SaveMeeting(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param, 
                meeting);

            return this.Json(ret);
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult UpdateUser(UserDTO user)
        {
            List<UserDTO> updatedUser = this.MeetingSetup.UpdateUser(
                this.Credentials, 
                this.MeetingSetup.GetProvider(this.Credentials), 
                this.Param, 
                user);

            return this.Json(updatedUser);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add session cookie.
        /// </summary>
        /// <param name="newId">
        /// The new id.
        /// </param>
        private void AddSessionCookie(string newId)
        {
            this.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", newId) { Domain = this.Settings.CookieDomain });
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

        #endregion
    }
}