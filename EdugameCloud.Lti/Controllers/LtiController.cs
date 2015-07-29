using Castle.Core.Logging;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API;

namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using DotNetOpenAuth.AspNet;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.BlackBoard;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Desire2Learn;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Core;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Core.OAuth;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.Models;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.Lti.OAuth.Canvas;
    using EdugameCloud.Lti.OAuth.Desire2Learn;
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
                            //if (provider.ToLower() == LmsProviderNames.Canvas)
                            //{
                            //    var param = this.GetSession(providerKey).LtiSession.LtiParam;
                            //    if (param.lms_user_login == "$Canvas.user.loginId")
                            //        throw new InvalidOperationException("[Canvas Authentication Error]. Please login to Canvas.");
                            //}

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
            LmsCompany companyLms = null;
            try
            {
                var lmsProviderName = settings.lmsProviderName;
                var session = this.GetSession(lmsProviderName);
                companyLms = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
                if (lmsUser == null)
                {
                    lmsUser = session.LmsUser ?? new LmsUser { LmsCompany = companyLms, UserId = param.lms_user_id, Username = GetUserNameOrEmail(param) };
                }

                lmsUser.AcConnectionMode = (AcConnectionMode)settings.acConnectionMode;
                lmsUser.PrimaryColor = settings.primaryColor;

                if (lmsUser.AcConnectionMode == AcConnectionMode.DontOverwriteLocalPassword)
                {
                    this.SetACPassword(session, param, settings.password);
                }
                else
                {
                    this.RemoveACPassword(param, session);
                }

                this.lmsUserModel.RegisterSave(lmsUser);
                return Json(OperationResult.Success(settings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SaveSettings", companyLms, ex);
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
                            isValid = !string.IsNullOrWhiteSpace(session.With(x => x.LtiSession).With(x => x.RestoredACPassword));
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
            var password = TempData["RestoredACPassword"] as string;
            var acUsesEmailAsLogin = TempData["ACUsesEmailAsLogin"] as bool? ?? false;
            var policies = TempData["ACPasswordPolicies"] as string;
            var usesSyncUsers = TempData["UseSynchronizedUsers"] as bool? ?? false;
            var useFLV = TempData["UseFLV"] as bool? ?? false;
            var useMP4 = TempData["UseMP4"] as bool? ?? false;
            string supportPageHtml = TempData["SupportPageHtml"] as string;

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

                if (string.IsNullOrWhiteSpace(password))
                {
                    password = session.LtiSession.RestoredACPassword;
                }

                meetingsJson = JsonConvert.SerializeObject(meetings);
                acUsesEmailAsLogin = credentials.ACUsesEmailAsLogin ?? false;
                policies = JsonConvert.SerializeObject(IoC.Resolve<IAdobeConnectAccountService>().GetPasswordPolicies(acProvider));
                usesSyncUsers = credentials.UseSynchronizedUsers;
                useFLV = credentials.UseFLV;
                useMP4 = credentials.UseMP4;
                supportPageHtml = credentials.GetSetting<string>(LmsCompanySettingNames.SupportPageHtml);
            }

            string version = typeof(LtiController).Assembly.GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));
            ViewBag.LtiVersion = version;
            ViewBag.MeetingsJson = meetingsJson;
            ViewBag.RestoredACPassword = password;
            ViewBag.ACUsesEmailAsLogin = acUsesEmailAsLogin;
            ViewBag.ACPasswordPolicies = policies;
            ViewBag.UseSynchronizedUsers = usesSyncUsers;
            ViewBag.UseFLV = useFLV;
            ViewBag.UseMP4 = useMP4;
            ViewBag.SupportPageHtml = supportPageHtml;
            return View("Index");
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
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.DeleteMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    scoId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
        public virtual ActionResult GetUsers(string lmsProviderName, string scoId, bool forceUpdate = false)
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
                    syncUsersService.SynchronizeUsers(lmsCompany, syncACUsers: false, scoIds: new[] { scoId });
                }
                List<LmsUserDTO> users = this.usersSetup.GetUsers(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param,
                    scoId,
                    out error,
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
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                string breezeSession = null;

                string url = this.meetingSetup.JoinMeeting(credentials, param, userSettings, scoId, ref breezeSession, this.GetAdobeConnectProvider(credentials));
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        public virtual ActionResult JoinMeetingMobile(string lmsProviderName, string scoId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                string breezeSession = null;

                string url = this.meetingSetup.JoinMeeting(credentials, param, userSettings, scoId, ref breezeSession, this.GetAdobeConnectProvider(credentials));

                if (string.IsNullOrWhiteSpace(breezeSession))
                    return Json(OperationResult.Error("Can't get Adobe Connect BreezeSession"), JsonRequestBehavior.AllowGet);

                return Json(OperationResult.Success(breezeSession), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinMeeting", credentials, ex);
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
        public virtual JsonResult LeaveMeeting(string lmsProviderName, string scoId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult result = this.meetingSetup.LeaveMeeting(credentials, param, scoId, this.GetAdobeConnectProvider(credentials));

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("LeaveMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
            var blackBoardProfile = ParseBlackBoardSharedInfo(lmsDomain);
            return this.View(
                "ProxyToolPassword",
                new ProxyToolPasswordModel
                {
                    LmsDomain = lmsDomain,
                    BlackBoardTitle =
                    string.IsNullOrWhiteSpace(blackBoardProfile.Name)
                    ? lmsDomain
                    : blackBoardProfile.Name,
                    LtiVersion = string.IsNullOrWhiteSpace(blackBoardProfile.LtiVersion) ? "2.0-July08" : blackBoardProfile.LtiVersion,
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
                if (lmsCompany != null)
                {
                    if (!string.IsNullOrWhiteSpace(lmsCompany.LmsDomain) && !lmsCompany.HasLmsDomain(param.lms_domain))
                    {
                        logger.ErrorFormat("LTI integration is already set for different domain. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                        this.ViewBag.Error = "This LTI integration is already set for different domain";
                        return this.View("Error");
                    }

                    var company = companyModel.GetOneById(lmsCompany.CompanyId).Value;
                    if ((company == null) || !company.IsActive())
                    {
                        logger.ErrorFormat("Company doesn't have any active license. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                        this.ViewBag.Error = "Sorry, your company doesn't have any active license. Please contact administration.";
                        return this.View("Error");
                    }
                }
                else
                {
                    logger.ErrorFormat("Adobe Connect integration is not set up. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                    this.ViewBag.Error = string.Format("Your Adobe Connect integration is not set up.");
                    return this.View("Error");
                }

                var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
                // NOTE: save in GetAdobeConnectProvider already this.SetAdobeConnectProvider(lmsCompany.Id, adobeConnectProvider);

                // TRICK: if LMS don't return user login - try to call lms' API to fetch user's info using user's LMS-ID.
                string email;
                string login;
                this.usersSetup.GetParamLoginAndEmail(param, lmsCompany, out email, out login);
                param.ext_user_username = login; // NOTE: is saved in session!

                var lmsUser = this.lmsUserModel.GetOneByUserIdOrUserNameOrEmailAndCompanyLms(param.lms_user_id, param.lms_user_login, param.lis_person_contact_email_primary, lmsCompany.Id);

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
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                OperationResult ret = this.meetingSetup.SaveMeeting(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meeting);

                return Json(ret);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateMeeting", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
            LmsCompany credentials = null;
            try
            {
                if (string.IsNullOrWhiteSpace(lmsProviderName))
                    throw new ArgumentException("lmsProviderName can't be empty", "lmsProviderName");
                if (string.IsNullOrWhiteSpace(scoId))
                    throw new ArgumentException("scoId can't be empty", "scoId");

                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;

                string error;
                LmsUserDTO updatedUser = null;
                if (user.guest_id.HasValue)
                {
                    updatedUser = this.usersSetup.UpdateGuest(
                        credentials,
                        this.GetAdobeConnectProvider(credentials),
                        session.LtiSession.LtiParam,
                        user,
                        scoId,
                        out error);
                }
                else
                {
                    updatedUser = this.usersSetup.UpdateUser(
                        credentials,
                        this.GetAdobeConnectProvider(credentials),
                        session.LtiSession.LtiParam,
                        user,
                        scoId,
                        out error);
                }

                if (string.IsNullOrEmpty(error))
                    return Json(OperationResult.Success(new[] { updatedUser }));

                return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("UpdateUser", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult SetDefaultRolesForNonParticipants(string lmsProviderName, string scoId)
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
                    scoId,
                    false,
                    out error);

                if (string.IsNullOrEmpty(error))
                    return Json(OperationResult.Success(updatedUsers));

                return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("SetDefaultRolesForNonParticipants", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
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
            var soapApi = IoC.Resolve<IBlackBoardApi>();
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
            catch (Exception ex)
            {
                logger.Error("ParseBlackBoardSharedInfo", ex);
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
        private static string GetUserNameOrEmail(LtiParamDTO param)
        {
            return string.IsNullOrWhiteSpace(param.lms_user_login) ? param.lis_person_contact_email_primary : param.lms_user_login;
        }

        /// <summary>
        /// The get LMS user settings for join.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="lmsCompany">
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
        private LmsUserSettingsDTO GetLmsUserSettingsForJoin(string lmsProviderName, LmsCompany lmsCompany, LtiParamDTO param, LmsUserSession session)
        {
            int connectionMode = 0;
            string primaryColor = null;
            LmsUser lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser != null)
            {
                connectionMode = (int)lmsUser.AcConnectionMode;
                primaryColor = lmsUser.PrimaryColor;
            }

            return new LmsUserSettingsDTO
            {
                acConnectionMode = connectionMode,
                primaryColor = primaryColor,
                lmsProviderName = lmsProviderName,
                password = session.LtiSession.RestoredACPassword,
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
            TempData["RestoredACPassword"] = session.LtiSession.RestoredACPassword;
            TempData["ACUsesEmailAsLogin"] = credentials.ACUsesEmailAsLogin;
            TempData["ACPasswordPolicies"] = JsonConvert.SerializeObject(IoC.Resolve<IAdobeConnectAccountService>().GetPasswordPolicies(acProvider));
            TempData["UseSynchronizedUsers"] = credentials.UseSynchronizedUsers;
            TempData["UseFLV"] = credentials.UseFLV;
            TempData["UseMP4"] = credentials.UseMP4;
            TempData["SupportPageHtml"] = credentials.GetSetting<string>(LmsCompanySettingNames.SupportPageHtml);

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

        /// <summary>
        /// The redirect to error.
        /// </summary>
        /// <param name="errorText">
        /// The error text.
        /// </param>
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
        private LmsUserSession SaveSession(LmsCompany company, LtiParamDTO param, LmsUser lmsUser)
        {
            var session = (lmsUser == null) ? null : this.userSessionModel.GetOneByCompanyAndUserAndCourse(company.Id, lmsUser.Id, param.course_id).Value;
            session = session ?? new LmsUserSession { LmsCompany = company, LmsUser = lmsUser, LmsCourseId = param.course_id };
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
            if (session != null)
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
        private void SetACPassword(LmsUserSession session, LtiParamDTO param, string adobeConnectPassword)
        {
            if (!string.IsNullOrWhiteSpace(adobeConnectPassword))
            {
                var sharedKey = AESGCM.NewKey();
                var sessionData = new LtiSessionDTO
                {
                    LtiParam = param,
                    ACPasswordData = AESGCM.SimpleEncrypt(adobeConnectPassword, sharedKey),
                    SharedKey = Convert.ToBase64String(sharedKey),
                };
                session.SessionData = JsonConvert.SerializeObject(sessionData);
                this.userSessionModel.RegisterSave(session);
            }
        }

        private void RemoveACPassword(LtiParamDTO param, LmsUserSession session)
        {
            if (!string.IsNullOrWhiteSpace(session.With(x => x.LtiSession).With(x => x.RestoredACPassword)))
            {
                var sessionData = new LtiSessionDTO
                {
                    LtiParam = param,
                    ACPasswordData = null,
                    SharedKey = null,
                };
                session.SessionData = JsonConvert.SerializeObject(sessionData);
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
            this.ViewBag.AcServer = credentials.AcServer.EndsWith("/")
                ? credentials.AcServer
                : credentials.AcServer + "/";

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