namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using Castle.Core.Logging;
    using DotNetOpenAuth.AspNet;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Desire2Learn;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Core;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Core.OAuth;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.Lti.OAuth.Canvas;
    using EdugameCloud.Lti.OAuth.Desire2Learn;
    using EdugameCloud.Lti.Utils;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Microsoft.Web.WebPages.OAuth;
    using Newtonsoft.Json;

    public partial class LtiController : Controller
    {
        private const string ExceptionMessage = "An exception is occured. Try again later or contact your administrator.";
        private const string ProviderKeyCookieName = "providerKey";

        #region Fields

        private static bool? isDebug;

        private readonly LmsCompanyModel lmsCompanyModel;
        private readonly CompanyModel companyModel;
        private readonly LmsUserSessionModel userSessionModel;
        private readonly LmsUserModel lmsUserModel;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly ILogger logger;
        private readonly ICanvasAPI canvasApi;
        private readonly IAdobeConnectUserService acUserService;
        private readonly LmsFactory lmsFactory;
        private readonly ISynchronizationUserService syncUsersService;

        #endregion

        #region Constructors and Destructors

        public LtiController(
            LmsCompanyModel lmsCompanyModel,
            CompanyModel companyModel,
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            ICanvasAPI canvasApi,
            IAdobeConnectUserService acUserService,
            LmsFactory lmsFactory,
            ISynchronizationUserService syncUsersService,
            ILogger logger)
        {
            this.lmsCompanyModel = lmsCompanyModel;
            this.companyModel = companyModel;
            this.userSessionModel = userSessionModel;
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.Settings = settings;
            this.usersSetup = usersSetup;
            this.canvasApi = canvasApi;
            this.logger = logger;
            this.acUserService = acUserService;
            this.lmsFactory = lmsFactory;
            this.syncUsersService = syncUsersService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the settings.
        /// </summary>
        public dynamic Settings { get; private set; }

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

        #endregion

        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here."),
        ActionName("callback")]
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

            try
            {
                if (string.IsNullOrEmpty(__provider__))
                {
                    logger.Error("[AuthenticationCallback] __provider__ parameter value is null or empty");
                    this.ViewBag.Error = "Could not find LMS information. Please, contact system administrator.";
                    return this.View("Error");
                }
                __provider__ = FixExtraDataIssue(__provider__);
                if (string.IsNullOrEmpty(providerKey))
                {
                    if (Request.Cookies.AllKeys.Contains(ProviderKeyCookieName))
                    {
                        providerKey = Request.Cookies[ProviderKeyCookieName].Value;
                    }
                    else
                    {
                        logger.Error("[AuthenticationCallback] providerKey parameter value is null and there is no cookie with such name");
                        this.ViewBag.Error = "Could not find session information for current user. Please, enable cookies or try to open LTI application in a different browser.";
                        return this.View("Error");
                    }
                }
                providerKey = FixExtraDataIssue(providerKey);
                string provider = __provider__;
                LmsUserSession session = this.GetSession(providerKey);
                var param = session.With(x => x.LtiSession).With(x => x.LtiParam);

                if (param.GetLtiProviderName(provider) == LmsProviderNames.Brightspace)
                {
                    var d2lService = IoC.Resolve<IDesire2LearnApiService>();

                    string scheme = Request.UrlReferrer.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
                    string authority = Request.UrlReferrer.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
                    var hostUrl = authority.Replace(scheme, string.Empty);

                    string username = null;
                    var company = session.With(x => x.LmsCompany);
                    var user = d2lService.GetApiObjects<WhoAmIUser>(Request.Url, hostUrl, String.Format(d2lService.WhoAmIUrlFormat, (string)Settings.D2LApiVersion), company);
                    if (string.IsNullOrEmpty(user.UniqueName))
                    {
                        var userInfo = d2lService.GetApiObjects<UserData>(Request.Url, hostUrl,
                            String.Format(d2lService.GetUserUrlFormat, (string)Settings.D2LApiVersion,
                                user.Identifier), company);
                        if (userInfo != null)
                        {
                            username = userInfo.UserName;
                        }
                    }
                    else
                    {
                        username = user.UniqueName;
                    }
                    string userId = Request.QueryString["x_a"];
                    string userKey = Request.QueryString["x_b"];
                    string token = null;
                    if (!userId.Contains(' ') && !userKey.Contains(' '))
                    {
                        token = userId + " " + userKey;
                    }
                    else
                    {
                        logger.ErrorFormat("[AuthenticationCallback] UserId:{0}, UserKey:{1}", userId, userKey);
                        this.ViewBag.Error = "Could not save user information in database. Please contact system administrator.";
                        return this.View("Error");
                    }

                    return AuthCallbackSave(providerKey, token, user.Identifier, username, "Error");
                }
                else
                {
                    try
                    {
                        AuthenticationResult result = OAuthWebSecurityWrapper.VerifyAuthentication(provider, this.Settings);
                        if (result.IsSuccessful)
                        {
                            if (provider.ToLower() == LmsProviderNames.Canvas)
                            {
                                if (param.lms_user_login == "$Canvas.user.loginId")
                                    throw new InvalidOperationException("[Canvas Authentication Error]. Please login to Canvas.");
                            }

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
                        this.ViewBag.Error = ex.ToString();
                    }
                }

                return this.View("Error");
            }
            catch (WarningMessageException ex)
            {
                logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", providerKey);
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", providerKey);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
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
        [HttpPost]
        public virtual JsonResult SaveSettings(LmsUserSettingsDTO settings)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var lmsProviderName = settings.lmsProviderName;
                var session = this.GetSession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    lmsUser = session.LmsUser ?? new LmsUser { LmsCompany = lmsCompany, UserId = param.lms_user_id, Username = GetUserNameOrEmail(param) };
                }

                var acConnectionMode = (AcConnectionMode)settings.acConnectionMode;
                lmsUser.PrimaryColor = settings.primaryColor;

                if (acConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
                {
                    var provider = GetAdobeConnectProvider(lmsCompany);
                    var couldSavePassword = UsersSetup.SetACPassword(provider, lmsCompany, lmsUser, param, acConnectionMode, settings.password);
                    if (!couldSavePassword)
                    {
                        return Json(OperationResult.Error("The password you provided is incorrect. Please try again."));
                    }
                }
                else
                {
                    lmsUser.SharedKey = null;
                    lmsUser.ACPasswordData = null;
                }

                lmsUser.AcConnectionMode = acConnectionMode;
                this.lmsUserModel.RegisterSave(lmsUser);
                return Json(OperationResult.Success(settings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSettings", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
        [HttpPost]
        public virtual JsonResult CheckPasswordBeforeJoin(string lmsProviderName)
        {
            LmsCompany companyLms = null;
            try
            {
                bool isValid = false;
                var session = this.GetSession(lmsProviderName);
                companyLms = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
                if (lmsUser != null)
                {
                    var mode = lmsUser.AcConnectionMode;
                    switch (mode)
                    {
                        case AcConnectionMode.DontOverwriteLocalPassword:
                            isValid = !string.IsNullOrWhiteSpace(lmsUser.ACPasswordData);
                            break;
                        default:
                            isValid = true;
                            break;
                    }
                }

                return Json(OperationResult.Success(isValid));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CheckPasswordBeforeJoin", companyLms, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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

        public virtual ActionResult GetExtJsPage(string primaryColor, string lmsProviderName, int acConnectionMode)
        {
            var meetingsJson = TempData["meetings"] as string;
            var policies = TempData["ACPasswordPolicies"] as string;
            var userFullName = TempData["CurrentUserFullName"] as string;
            LicenceSettingsDto settings = TempData["LicenceSettings"] as LicenceSettingsDto;

            if (string.IsNullOrWhiteSpace(meetingsJson))
            {
                LmsUserSession session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var acProvider = this.GetAdobeConnectProvider(credentials);
                var meetings = this.meetingSetup.GetMeetings(
                    credentials,
                    acProvider,
                    param);

                meetingsJson = JsonConvert.SerializeObject(meetings);                
                policies = JsonConvert.SerializeObject(IoC.Resolve<IAdobeConnectAccountService>().GetPasswordPolicies(acProvider));
                userFullName = param.lis_person_name_full;
                settings = LicenceSettingsDto.Build(credentials);
            }

            string version = typeof(LtiController).Assembly.GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));
            ViewBag.LtiVersion = version;
            ViewBag.MeetingsJson = meetingsJson;
            ViewBag.ACPasswordPolicies = policies;
            // TRICK:
            // BB contains: lis_person_name_full:" Blackboard  Administrator"
            ViewBag.CurrentUserFullName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline);
            ViewBag.LmsLicenceSettings = settings;
            return View("Index");
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
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                List<TemplateDTO> templates = this.meetingSetup.GetTemplates(
                    this.GetAdobeConnectProvider(session.LmsCompany),
                    session.LmsCompany.ACTemplateScoId);

                return Json(OperationResult.Success(templates));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetTemplates", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
        public virtual ActionResult GetUsers(string lmsProviderName, int meetingId, bool forceUpdate = false)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error;
                var service = lmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProvider.Id);

                if (forceUpdate && lmsCompany.UseSynchronizedUsers
                    && service != null
                    && service.CanRetrieveUsersFromApiForCompany(lmsCompany)
                    && lmsCompany.LmsCourseMeetings != null
                    && lmsCompany.LmsCourseMeetings.Any(x => x.LmsMeetingType != (int)LmsMeetingType.OfficeHours))
                {
                    syncUsersService.SynchronizeUsers(lmsCompany, syncACUsers: false, meetingIds: new[] { meetingId });
                }

                var users = this.usersSetup.GetUsers(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param,
                    meetingId,
                    out error,
                    null,
                    forceUpdate);

                if (string.IsNullOrWhiteSpace(error))
                {
                    return Json(OperationResult.Success(users));
                }

                return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetUsers", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
        
        public virtual ActionResult JoinMeeting(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                string breezeSession = null;

                string url = this.meetingSetup.JoinMeeting(credentials, param, meetingId,
                    ref breezeSession, this.GetAdobeConnectProvider(credentials));
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                this.ViewBag.Message = ex.Message;
                // TRICK: to increase window height
                this.ViewBag.DebugError = "eSyncTraining Inc.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "JoinMeeting exception. Id:{0}.", meetingId);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        public virtual ActionResult JoinMeetingMobile(string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var provider = GetAdobeConnectProvider(lmsCompany);
                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new WarningMessageException(string.Format("No user with id {0} found.", param.lms_user_id));
                }

                var principalInfo = !string.IsNullOrWhiteSpace(lmsUser.PrincipalId) ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo : null;
                Principal registeredUser = principalInfo != null ? principalInfo.Principal : null;
                string breezeSession = null;

                if (registeredUser != null)
                {
                    breezeSession = MeetingSetup.ACLogin(
                        lmsCompany,
                        param,
                        lmsUser,
                        registeredUser,
                        provider);
                }
                else
                {
                    var message = string.Format(
                        "No user with principal id {0} found in Adobe Connect.", lmsUser.PrincipalId ?? string.Empty);
                    logger.Error(message);
                    throw new WarningMessageException(message);
                }

                if (string.IsNullOrWhiteSpace(breezeSession))
                    return Json(OperationResult.Error("Can't get Adobe Connect BreezeSession"), JsonRequestBehavior.AllowGet);

                return Json(OperationResult.Success(breezeSession), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinMeeting", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
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
        [HttpPost]
        public virtual JsonResult LeaveMeeting(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.LeaveMeeting(credentials, param, meetingId, this.GetAdobeConnectProvider(credentials));

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("LeaveMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [ActionName("login")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult LoginWithProvider(string provider, LtiParamDTO param)
        {
            try
            {
                //var sw = Stopwatch.StartNew();

                string lmsProvider = param.GetLtiProviderName(provider);
                if (IoC.Resolve<LmsProviderModel>().GetOneByName(lmsProvider).Value == null)
                {
                    logger.ErrorFormat("Invalid LMS provider name. LMS Provider Name:{0}. oauth_consumer_key:{1}.", lmsProvider, param.oauth_consumer_key);
                    this.ViewBag.Error = "Review LTI integration. Possible you have invalid External Tool URL.";
                    return this.View("Error");
                }

                if (lmsProvider.ToLower() == LmsProviderNames.Brightspace && !string.IsNullOrEmpty(param.user_id))
                {
                    logger.InfoFormat("[D2L login attempt]. Original user_id: {0}. oauth_consumer_key:{1}.", param.user_id, param.oauth_consumer_key);
                    var parsedIdArray = param.user_id.Split('_');
                    // temporary fix
                    if (parsedIdArray.Length > 1)
                    {
                        param.user_id = parsedIdArray.Last();
                    }
                }
                
                LmsCompany lmsCompany = this.lmsCompanyModel.GetOneByProviderAndConsumerKey(lmsProvider, param.oauth_consumer_key).Value;
                string validationError = ValidateLmsLicense(lmsCompany, param);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    this.ViewBag.Error = validationError;
                    return this.View("Error");
                }
               
                var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
                // NOTE: save in GetAdobeConnectProvider already this.SetAdobeConnectProvider(lmsCompany.Id, adobeConnectProvider);

                // TRICK: if LMS don't return user login - try to call lms' API to fetch user's info using user's LMS-ID.
                param.ext_user_username = usersSetup.GetParamLogin(param, lmsCompany); ; // NOTE: is saved in session!

                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                LmsUserSession session = this.SaveSession(lmsCompany, param, lmsUser);
                var key = session.Id.ToString();

                this.meetingSetup.SetupFolders(lmsCompany, adobeConnectProvider);

                if (BltiProviderHelper.VerifyBltiRequest(lmsCompany, logger, () => this.ValidateLMSDomainAndSaveIfNeeded(param, lmsCompany)) || this.IsDebug)
                {
                    Principal acPrincipal = null;

                    switch (lmsProvider.ToLower())
                    {
                        case LmsProviderNames.Canvas:
                            if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token) || canvasApi.IsTokenExpired(lmsCompany.LmsDomain, lmsUser.Token))
                            {
                                this.StartOAuth2Authentication(provider, key, param);
                                return null;
                            }

                            if (lmsCompany.AdminUser == null)
                            {
                                this.logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsCompany.Id);
                                this.ViewBag.Message = "Sorry, LMS Admin is not set for current license. Please contact administrator.";
                                return this.View("~/Views/Lti/LtiError.cshtml");
                            }

                            acPrincipal = acUserService.GetOrCreatePrincipal(
                                adobeConnectProvider,
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.lis_person_name_given,
                                param.lis_person_name_family,
                                lmsCompany);
                            break;
                        case LmsProviderNames.Brightspace:
                            //todo: review. Probably we need to redirect to auth url everytime for overwriting tokens if user logs in under different roles
                            if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                            {
                                string schema = Request.GetScheme();

                                var d2lService = IoC.Resolve<IDesire2LearnApiService>();
                                string returnUrl = this.Url.AbsoluteAction(
                                    "callback",
                                    "Lti",
                                    new { __provider__ = provider },
                                    schema);
                                Response.Cookies.Add(new HttpCookie(ProviderKeyCookieName, key));
                                return Redirect(
                                    d2lService
                                        .GetTokenRedirectUrl(new Uri(returnUrl), param.lms_domain, lmsCompany)
                                        .AbsoluteUri);
                            }

                            if (lmsCompany.AdminUser == null)
                            {
                                this.logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsCompany.Id);
                                this.ViewBag.Message = "Sorry, LMS Admin is not set for current license. Please contact administrator.";
                                return this.View("~/Views/Lti/LtiError.cshtml");
                            }

                            acPrincipal = acUserService.GetOrCreatePrincipal(
                                adobeConnectProvider,
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.lis_person_name_given,
                                param.lis_person_name_family,
                                lmsCompany);
                            break;
                        case LmsProviderNames.BrainHoney:
                        case LmsProviderNames.Blackboard:
                        case LmsProviderNames.Moodle:
                        case LmsProviderNames.Sakai:
                            acPrincipal = acUserService.GetOrCreatePrincipal(
                                adobeConnectProvider,
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.lis_person_name_given,
                                param.lis_person_name_family,
                                lmsCompany);
                            if (lmsUser == null)
                            {
                                lmsUser = new LmsUser
                                {
                                    LmsCompany = lmsCompany,
                                    UserId = param.lms_user_id,
                                    Username = GetUserNameOrEmail(param),
                                    PrincipalId = acPrincipal != null ? acPrincipal.PrincipalId : null,
                                };
                                this.lmsUserModel.RegisterSave(lmsUser);

                                // TRICK: save lmsUser to session!
                                // TRICK: remove the previous session - with [lmsUserId]==NULL
                                this.userSessionModel.RegisterDelete(session, flush: true);
                                session = this.SaveSession(lmsCompany, param, lmsUser);
                                key = session.Id.ToString();
                            }

                            break;
                    }

                    if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
                    {
                        lmsUser.PrincipalId = acPrincipal.PrincipalId;
                        this.lmsUserModel.RegisterSave(lmsUser);
                    }

                    if (acPrincipal == null)
                    {
                        this.logger.ErrorFormat("[LoginWithProvider] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.", lmsCompany.Id, lmsUser.Id, param.lms_user_login);
                        throw new WarningMessageException("Sorry, Adobe Connect account does not exist for you. Please contact administrator.");
                    }

                    //sw.Stop();
                    //var time = sw.Elapsed;

                    return this.RedirectToExtJs(session, lmsUser, key);
                }

                logger.ErrorFormat("Invalid LTI request. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.Error = "Invalid LTI request";
                return this.View("Error");
            }
            catch (WarningMessageException ex)
            {
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        [HttpPost]
        public virtual JsonResult UpdateMeetingAndReturnLmsUsers(string lmsProviderName, MeetingDTO meeting)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting,
                    true);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeetingAndReturnLmsUsers", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpPost]
        public virtual JsonResult SetDefaultRolesForNonParticipants(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error = null;
                List<LmsUserDTO> updatedUsers = this.usersSetup.SetDefaultRolesForNonParticipants(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meetingId,
                    false,
                    out error);

                //if (string.IsNullOrEmpty(error))
                    return Json(OperationResult.Success(error, updatedUsers));

                //return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SetDefaultRolesForNonParticipants", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        #endregion

        #region Methods

        private string ValidateLmsLicense(LmsCompany lmsCompany, LtiParamDTO param)
        {
            if (lmsCompany != null)
            {
                if (!string.IsNullOrWhiteSpace(lmsCompany.LmsDomain) && !lmsCompany.HasLmsDomain(param.lms_domain))
                {
                    logger.ErrorFormat("LTI integration is already set for different domain. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return "This LTI integration is already set for different domain";
                }

                if (!lmsCompany.IsActive)
                {
                    logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return "LMS License is not active. Please contact administrator.";
                }

                var company = companyModel.GetOneById(lmsCompany.CompanyId).Value;
                if ((company == null) || !company.IsActive())
                {
                    logger.ErrorFormat("Company doesn't have any active license. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                    return "Sorry, your company doesn't have any active license. Please contact administrator.";
                }
            }
            else
            {
                logger.ErrorFormat("Adobe Connect integration is not set up. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                return string.Format("Your Adobe Connect integration is not set up.");
            }

            return null;
        }

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
        /// The get user name or email.
        /// </summary>
        /// <param name="param">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetUserNameOrEmail(LtiParamDTO param)
        {
            return string.IsNullOrWhiteSpace(param.lms_user_login) ? param.lis_person_contact_email_primary : param.lms_user_login;
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
        private bool ValidateLMSDomainAndSaveIfNeeded(LtiParamDTO param, LmsCompany credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.LmsDomain))
            {
                credentials.LmsDomain = param.lms_domain;
                this.lmsCompanyModel.RegisterSave(credentials, true);
                return true;
            }

            // TODO: !!! WWW section!!!
            return credentials.HasLmsDomain(param.lms_domain);

            // TODO: !!! WWW section!!!
            //return param.lms_domain.ToLower().Replace("www.", string.Empty).Equals(credentials.LmsDomain.Replace("www.", string.Empty), StringComparison.OrdinalIgnoreCase);
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
            string schema = Request.GetScheme();

            string returnUrl = this.Url.AbsoluteAction(
                        "callback",
                        "Lti",
                        new { __provider__ = provider, providerKey },
                        schema);
            switch (provider)
            {
                case LmsProviderNames.Canvas:
                    returnUrl = UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);

                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, providerKey);
                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;
                case LmsProviderNames.Brightspace:
                    UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);

                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;

            }
        }

        private ActionResult AuthCallbackSave(string providerKey, string token, string userId, string username, string viewName)
        {
            LmsUser lmsUser = null;
            LmsUserSession session = this.GetSession(providerKey);
            var company = session.With(x => x.LmsCompany);
            var param = session.With(x => x.LtiSession).With(x => x.LtiParam);
            if (!string.IsNullOrEmpty(token))
            {
                string userName = username;
                if (string.IsNullOrWhiteSpace(username) && (providerKey.ToLower() == LmsProviderNames.Canvas) && (param.lms_user_login == "$Canvas.user.loginId"))
                {
                    logger.Warn("[Canvas Auth Issue]. lms_user_login == '$Canvas.user.loginId'");
                    LmsUserDTO user = canvasApi.GetUser(company.LmsDomain, token, userId);
                    if (user != null)
                        userName = user.login_id;
                }

                if (string.IsNullOrWhiteSpace(username))
                    userName = GetUserNameOrEmail(param);

                lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(userId, company.Id).Value 
                    ?? new LmsUser { UserId = userId, LmsCompany = company, Username = userName };
                lmsUser.Username = userName;
                lmsUser.Token = token;
                if (lmsUser.IsTransient())
                {
                    this.SaveSessionUser(session, lmsUser);
                }

                // TRICK: during loginwithprovider we redirect to Oauth before we create AC principal - so we need to do it here
                Principal acPrincipal = acUserService.GetOrCreatePrincipal(
                                this.GetAdobeConnectProvider(company),
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.lis_person_name_given,
                                param.lis_person_name_family,
                                company);
                if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
                {
                    lmsUser.PrincipalId = acPrincipal.PrincipalId;
                }

                this.lmsUserModel.RegisterSave(lmsUser);

                if (acPrincipal == null)
                {
                    this.logger.ErrorFormat("[AuthCallbackSave] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.", company.Id, lmsUser.Id, param.lms_user_login);
                    throw new WarningMessageException("Sorry, Adobe Connect account does not exist for you. Please contact administrator.");
                }
            }

            if (company != null)
            {
                if (company.AdminUser == null)//this.IsAdminRole(providerKey))
                {
                    bool currentUserIsAdmin = false;
                    if (providerKey.ToLower() == LmsProviderNames.Brightspace)
                    {
                        if (!string.IsNullOrEmpty(param.ext_d2l_role))
                        {
                            currentUserIsAdmin = param.ext_d2l_role.ToLower().Contains("administrator");
                        }
                        else
                        {
                            //we need somehow to distinct teacher and admin user here
                            //todo: review this approach, extract to service
                            var d2lService = IoC.Resolve<IDesire2LearnApiService>();
                            //get enrollments - this information contains user roles
                            var enrollmentsList = new List<OrgUnitUser>();
                            var tokens = token.Split(' ');
                            PagedResultSet<OrgUnitUser> enrollments = null;
                            do
                            {
                                enrollments = d2lService.GetApiObjects<PagedResultSet<OrgUnitUser>>(tokens[0], tokens[1],
                                    param.lms_domain,
                                    String.Format(d2lService.EnrollmentsUrlFormat,
                                        (string) Settings.D2LApiVersion, param.context_id) +
                                    (enrollments != null ? "?bookmark=" + enrollments.PagingInfo.Bookmark : string.Empty),
                                    company);
                                if (enrollments != null || enrollments.Items == null)
                                {
                                    enrollmentsList.AddRange(enrollments.Items);
                                }
                            } while (enrollments != null && enrollments.PagingInfo.HasMoreItems);
                            currentUserIsAdmin = enrollments != null && enrollments.Items != null
                                                 && enrollments.Items.Any(x => x.Role.Name.ToLower().Contains("admin"));
                        }
                    }
                    else
                    {
                        currentUserIsAdmin = IsAdminRole(param);
                    }

                    if (currentUserIsAdmin)
                    {
                        company.AdminUser = lmsUser;
                        lmsCompanyModel.RegisterSave(company);
                    }
                    else
                    {
                        this.logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", company.Id);
                        throw new WarningMessageException("Sorry, LMS Admin is not set for current license. Please contact administrator.");
                    }
                }
                
                return this.RedirectToExtJs(session, lmsUser, providerKey);
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
        private ActionResult RedirectToExtJs(LmsUserSession session, LmsUser lmsUser, string providerName)
        {
            var credentials = session.LmsCompany;
            var primaryColor = lmsUser.PrimaryColor;
            primaryColor = !string.IsNullOrWhiteSpace(primaryColor) ? primaryColor : (credentials.PrimaryColor ?? string.Empty);

            var param = session.LtiSession.LtiParam;
            var acProvider = this.GetAdobeConnectProvider(credentials);
            var meetings = this.meetingSetup.GetMeetings(
                credentials,
                acProvider,
                param);

            TempData["meetings"] = JsonConvert.SerializeObject(meetings);
            TempData["LicenceSettings"] = LicenceSettingsDto.Build(credentials);
            TempData["CurrentUserFullName"] = param.lis_person_name_full;
            TempData["ACPasswordPolicies"] = JsonConvert.SerializeObject(IoC.Resolve<IAdobeConnectAccountService>().GetPasswordPolicies(acProvider));

            return RedirectToAction("GetExtJsPage", "Lti", new { primaryColor = primaryColor, lmsProviderName = providerName, acConnectionMode = (int)lmsUser.AcConnectionMode });
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
        private bool IsAdminRole(LtiParamDTO param)
        {
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
        /// The <see cref="LmsCompany"/>.
        /// </returns>
        private LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (this.IsDebug && session == null)
            {
                session = this.userSessionModel.GetByIdWithRelated(Guid.Empty).Value;
            }

            if (session == null)
            {
                this.RedirectToError("Session timed out. Please refresh the page.");
                return null;
            }

            return session;
        }

        private IAdobeConnectProxy GetAdobeConnectProvider(LmsCompany lmsCompany)
        {
            IAdobeConnectProxy provider = null;
            if (lmsCompany != null)
            {
                provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] as IAdobeConnectProxy;
                if (provider == null)
                {
                    provider = this.meetingSetup.GetProvider(lmsCompany);
                    this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] = provider;
                }
            }

            return provider;
        }

        private void RedirectToError(string errorText)
        {
            this.Response.Clear();
            this.Response.Write(string.Format("{{ \"isSuccess\": \"false\", \"message\": \"{0}\" }}", errorText));
            this.Response.End();
        }

        /// <summary>
        ///     The regenerate id.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        //private void RegenerateId()
        //{
        //    HttpContext httpContext = System.Web.HttpContext.Current;
        //    var manager = new SessionIDManager();
        //    string oldId = manager.GetSessionID(httpContext);
        //    string newId = manager.CreateSessionID(httpContext);

        //    bool isAdd, isRedirected;
        //    manager.SaveSessionID(httpContext, newId, out isRedirected, out isAdd);
        //    HttpApplication application = httpContext.ApplicationInstance;
        //    HttpModuleCollection modules = application.Modules;
        //    var ssm = (SessionStateModule)modules.Get("Session");
        //    FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        //    SessionStateStoreProviderBase store = null;
        //    FieldInfo requiredIdField = null, requiredLockIdField = null, requiredStateNotFoundField = null;
        //    foreach (FieldInfo field in fields)
        //    {
        //        if (field.Name.Equals("_store"))
        //        {
        //            store = (SessionStateStoreProviderBase)field.GetValue(ssm);
        //        }

        //        if (field.Name.Equals("_rqId"))
        //        {
        //            requiredIdField = field;
        //        }

        //        if (field.Name.Equals("_rqLockId"))
        //        {
        //            requiredLockIdField = field;
        //        }

        //        if (field.Name.Equals("_rqSessionStateNotFound"))
        //        {
        //            requiredStateNotFoundField = field;
        //        }
        //    }

        //    if (requiredLockIdField != null)
        //    {
        //        object lockId = requiredLockIdField.GetValue(ssm);
        //        if (lockId != null && oldId != null && store != null)
        //        {
        //            store.ReleaseItemExclusive(httpContext, oldId, lockId);
        //        }
        //    }

        //    if (requiredStateNotFoundField != null)
        //    {
        //        requiredStateNotFoundField.SetValue(ssm, true);
        //    }

        //    if (requiredIdField != null)
        //    {
        //        requiredIdField.SetValue(ssm, newId);
        //    }
        //}

        private LmsUserSession SaveSession(LmsCompany company, LtiParamDTO param, LmsUser lmsUser)
        {
            var session = (lmsUser == null) ? null : this.userSessionModel.GetOneByCompanyAndUserAndCourse(lmsUser.Id, param.course_id).Value;
            session = session ?? new LmsUserSession { LmsCompany = company, LmsUser = lmsUser, LmsCourseId = param.course_id };
            var sessionData = new LtiSessionDTO { LtiParam = param };
            if (lmsUser != null && lmsUser.AcConnectionMode == AcConnectionMode.DontOverwriteLocalPassword 
                && session.LtiSession != null)
            {
                var oldSessionData = session.LtiSession;
                sessionData.ACPasswordData = oldSessionData.ACPasswordData;
                sessionData.SharedKey = oldSessionData.SharedKey;
            }
            session.SessionData = JsonConvert.SerializeObject(sessionData);
            this.userSessionModel.RegisterSave(session, flush: true);

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
            if (session != null)
            {
                session.LmsUser = lmsUser;
                this.userSessionModel.RegisterSave(session);
            }
        }

        private ActionResult LoginToAC(string realUrl, string breezeSession, LmsCompany credentials)
        {
            if (!credentials.LoginUsingCookie.GetValueOrDefault())
            {
                return this.Redirect(realUrl);
            }

            this.ViewBag.MeetingUrl = realUrl;
            this.ViewBag.BreezeSession = breezeSession;
            this.ViewBag.AcServer = credentials.AcServer + "/";

            return this.View("LoginToAC");
        }

        private string GetOutputErrorMessage(string methodName, LmsCompany credentials, Exception ex)
        {
            string lmsInfo = (credentials != null)
                ? string.Format(" LmsCompany ID: {0}. Lms License Title: {1}. Lms Domain: {2}. AC Server: {3}.", credentials.Id, credentials.Title, credentials.LmsDomain, credentials.AcServer)
                : string.Empty;

            logger.Error(methodName + lmsInfo, ex);

            var forcePassMessage = ex as WarningMessageException;
            if (forcePassMessage != null)
                return forcePassMessage.Message;

            return IsDebug
                ? "An exception is occured. " + ex.ToString()
                : ExceptionMessage;
        }

        #endregion

    }

}