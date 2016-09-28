namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using Core.Domain.Entities;
    using DotNetOpenAuth.AspNet;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Lti.API;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.API.Canvas;
    using EdugameCloud.Lti.API.Desire2Learn;
    using EdugameCloud.Lti.Constants;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Core.Constants;
    using EdugameCloud.Lti.Core.OAuth;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;
    using EdugameCloud.Lti.OAuth;
    using EdugameCloud.Lti.OAuth.Canvas;
    using EdugameCloud.Lti.OAuth.Desire2Learn;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core;
    using Esynctraining.Core.Caching;
    using Esynctraining.Core.Domain;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Microsoft.Web.WebPages.OAuth;
    using Newtonsoft.Json;

    public partial class LtiController : Controller
    {
        private const string ProviderKeyCookieName = "providerKey";

        #region Fields

        private static bool? isDebug;

        private readonly LmsCompanyModel lmsCompanyModel;
        private readonly LmsUserSessionModel userSessionModel;
        private readonly LmsUserModel lmsUserModel;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly ILogger logger;
        private readonly IAdobeConnectUserService acUserService;
        private readonly ICache _cache;

        #endregion

        private ICanvasAPI CanvasApi
        {
            get { return IoC.Resolve<ICanvasAPI>(); }
        }

        private ISynchronizationUserService SynchronizationUserService
        {
            get { return IoC.Resolve<ISynchronizationUserService>(); }
        }

        private LmsFactory LmsFactory
        {
            get { return IoC.Resolve<LmsFactory>(); }
        }

        private LanguageModel LanguageModel
        {
            get { return IoC.Resolve<LanguageModel>(); }
        }

        private CompanyModel CompanyModel
        {
            get { return IoC.Resolve<CompanyModel>(); }
        }

        private API.AdobeConnect.IAdobeConnectAccountService AdobeConnectAccountService
        {
            get { return IoC.Resolve<API.AdobeConnect.IAdobeConnectAccountService>(); }
        }

        private LmsProviderModel LmsProviderModel
        {
            get { return IoC.Resolve<LmsProviderModel>(); }
        }

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        #region Constructors and Destructors

        public LtiController(
            LmsCompanyModel lmsCompanyModel,
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectUserService acUserService,
            ILogger logger,
            ICache cache)
        {
            this.lmsCompanyModel = lmsCompanyModel;
            this.userSessionModel = userSessionModel;
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.Settings = settings;
            this.usersSetup = usersSetup;
            this.logger = logger;
            this.acUserService = acUserService;
            _cache = cache;
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

        protected override JsonResult Json(object data, string contentType,
                System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
            };
        }

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
                    this.ViewBag.Error = Resources.Messages.NoLmsInformation;
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
                        this.ViewBag.Error = Resources.Messages.NoSessionInformation;
                        return this.View("Error");
                    }
                }
                providerKey = FixExtraDataIssue(providerKey);
                string provider = __provider__;
                LmsUserSession session = GetSession(providerKey);
                var param = session.With(x => x.LtiSession).With(x => x.LtiParam);

                if (param.GetLtiProviderName(provider) == LmsProviderNames.Brightspace)
                {
                    var d2lService = IoC.Resolve<IDesire2LearnApiService>();

                    string scheme = Request.UrlReferrer.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
                    string authority = Request.UrlReferrer.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
                    var hostUrl = authority.Replace(scheme, string.Empty);

                    string username = null;
                    var company = session.With(x => x.LmsCompany);
                    var user = d2lService.GetApiObjects<WhoAmIUser>(Request.Url, hostUrl, String.Format(d2lService.WhoAmIUrlFormat, (string)Settings.BrightspaceApiVersion), company);
                    if (string.IsNullOrEmpty(user.UniqueName))
                    {
                        var userInfo = d2lService.GetApiObjects<UserData>(Request.Url, hostUrl,
                            String.Format(d2lService.GetUserUrlFormat, (string)Settings.BrightspaceApiVersion,
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
                        this.ViewBag.Error = Resources.Messages.CanSaveToDb;
                        return this.View("Error");
                    }

                    return AuthCallbackSave(providerKey, provider, token, user.Identifier, username, "Error");
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

                            return AuthCallbackSave(providerKey, provider,
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
            catch (Core.WarningMessageException ex)
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
                var session = GetReadOnlySession(lmsProviderName);
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
                    var couldSavePassword = UsersSetup.SetACPassword(provider, lmsCompany, lmsUser, param, settings.password);
                    if (!couldSavePassword)
                    {
                        return Json(OperationResult.Error(Resources.Messages.IncorrectAcPassword));
                    }
                }
                else
                {
                    lmsUser.SharedKey = null;
                    lmsUser.ACPasswordData = null;
                }

                lmsUser.AcConnectionMode = acConnectionMode;
                this.lmsUserModel.RegisterSave(lmsUser);
                return Json(OperationResultWithData<LmsUserSettingsDTO>.Success(settings));
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
                var session = GetReadOnlySession(lmsProviderName);
                companyLms = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, companyLms.Id).Value;
                if (lmsUser != null)
                {
                    var mode = lmsUser.AcConnectionMode;
                    switch (mode)
                    {
                        case AcConnectionMode.DontOverwriteLocalPassword:
                            isValid = !string.IsNullOrWhiteSpace(lmsUser.ACPassword);
                            break;
                        default:
                            isValid = true;
                            break;
                    }
                }

                return Json(OperationResultWithData<bool>.Success(isValid));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CheckPasswordBeforeJoin", companyLms, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        //public virtual ActionResult GetHtmlPage(string path)
        //{
        //    return new FilePathResult(path, "text/html");
        //}

        public virtual ActionResult GetExtJsPage(string primaryColor, string lmsProviderName, int acConnectionMode, bool disableCacheBuster = true)
        {
            LtiViewModelDto model = TempData["lti-index-model"] as LtiViewModelDto;

            // TRICK: to change lang inside
            LmsUserSession session = GetReadOnlySession(lmsProviderName);

            if (model == null)
            {
                model = BuildModel(session);
            }
            
            return View("Index", model);
        }
                
        [HttpPost]
        public virtual ActionResult GetUsers(string lmsProviderName, int meetingId, bool forceUpdate = false)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error;
                var service = LmsFactory.GetUserService((LmsProviderEnum)lmsCompany.LmsProviderId);

                if (forceUpdate && lmsCompany.UseSynchronizedUsers
                    && service != null
                    && service.CanRetrieveUsersFromApiForCompany(lmsCompany)
                    && lmsCompany.LmsCourseMeetings != null)
                {
                    SynchronizationUserService.SynchronizeUsers(lmsCompany, syncACUsers: false, meetingIds: new[] { meetingId });
                }

                IList<LmsUserDTO> users = this.usersSetup.GetUsers(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param,
                    meetingId,
                    out error,
                    null);

                if (string.IsNullOrWhiteSpace(error))
                {
                    return Json(OperationResultWithData<IList<LmsUserDTO>>.Success(users));
                }

                return Json(OperationResult.Error(error));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetUsers", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //public virtual ActionResult Index(LtiParamDTO model)
        //{
        //    string providerName = model.tool_consumer_info_product_family_code;
        //    return this.LoginWithProvider(providerName, model);
        //}
        
        public virtual ActionResult JoinMeeting(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                string breezeSession = null;

                string url = this.meetingSetup.JoinMeeting(credentials, param, meetingId,
                    ref breezeSession, this.GetAdobeConnectProvider(credentials));
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (Core.WarningMessageException ex)
            {
                this.ViewBag.Message = ex.Message;
                // TRICK: to increase window height
                this.ViewBag.DebugError = "eSyncTraining Inc.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "JoinMeeting exception. Id:{0}. SessionID: {1}.", meetingId, lmsProviderName);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        public virtual ActionResult JoinMeetingMobile(string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var provider = GetAdobeConnectProvider(lmsCompany);
                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException(string.Format(Resources.Messages.NoUserFound, param.lms_user_id));
                }

                var principalInfo = !string.IsNullOrWhiteSpace(lmsUser.PrincipalId) ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo : null;
                Principal registeredUser = principalInfo != null ? principalInfo.Principal : null;
                string breezeSession = null;

                if (registeredUser != null)
                {
                    breezeSession = meetingSetup.ACLogin(
                        lmsCompany,
                        param,
                        lmsUser,
                        registeredUser,
                        provider);
                }
                else
                {
                    var message = string.Format(
                        Resources.Messages.NoUserByPrincipalIdFound, lmsUser.PrincipalId ?? string.Empty);
                    logger.Error(message);
                    throw new Core.WarningMessageException(message);
                }

                if (string.IsNullOrWhiteSpace(breezeSession))
                    return Json(OperationResult.Error(Resources.Messages.CanNotGetBreezeSession), JsonRequestBehavior.AllowGet);

                return Json(OperationResultWithData<string>.Success(breezeSession), JsonRequestBehavior.AllowGet);
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
                var session = GetReadOnlySession(lmsProviderName);
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
            var methodTime = Stopwatch.StartNew();
            var trace = new StringBuilder();
            try
            {
                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es");
                //throw new InvalidOperationException("trick test");

                string lmsProvider = param.GetLtiProviderName(provider);
                LmsProvider providerInstance = IoC.Resolve<LmsProviderModel>().GetByName(lmsProvider);
                if (providerInstance == null)
                {
                    logger.ErrorFormat("Invalid LMS provider name. LMS Provider Name:{0}. oauth_consumer_key:{1}.", lmsProvider, param.oauth_consumer_key);
                    this.ViewBag.Error = Resources.Messages.LtiExternalToolUrl;
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
                
                var sw = Stopwatch.StartNew();

                LmsCompany lmsCompany = this.lmsCompanyModel.GetOneByProviderAndConsumerKey(providerInstance.Id, param.oauth_consumer_key).Value;

                if (lmsCompany != null)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(lmsCompany.LanguageId).TwoLetterCode);
                }

                string validationError = ValidateLmsLicense(lmsCompany, param);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    this.ViewBag.Error = validationError;
                    return this.View("Error");
                }

                sw.Stop();
                trace.AppendFormat("GetOneByProviderAndConsumerKey and ValidateLmsLicense: time: {0}.\r\n", sw.Elapsed.ToString());


                
                sw = Stopwatch.StartNew();

                var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany, forceReCreate: true);
                // NOTE: save in GetAdobeConnectProvider already this.SetAdobeConnectProvider(lmsCompany.Id, adobeConnectProvider);

                sw.Stop();
                trace.AppendFormat("GetAdobeConnectProvider: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                // TRICK: if LMS don't return user login - try to call lms' API to fetch user's info using user's LMS-ID.
                param.ext_user_username = usersSetup.GetParamLogin(param, lmsCompany); ; // NOTE: is saved in session!

                sw.Stop();
                trace.AppendFormat("GetParamLogin: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                sw.Stop();
                trace.AppendFormat("GetOneByUserIdAndCompanyLms: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                LmsUserSession session = this.SaveSession(lmsCompany, param, lmsUser);
                var key = session.Id.ToString();

                sw.Stop();
                trace.AppendFormat("SaveSession: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                this.meetingSetup.SetupFolders(lmsCompany, adobeConnectProvider);

                sw.Stop();
                trace.AppendFormat("meetingSetup.SetupFolders: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                if (BltiProviderHelper.VerifyBltiRequest(lmsCompany, () => this.ValidateLMSDomainAndSaveIfNeeded(param, lmsCompany)) || this.IsDebug)
                {

                    sw.Stop();
                    trace.AppendFormat("VerifyBltiRequest: time: {0}.\r\n", sw.Elapsed.ToString());
                    sw = Stopwatch.StartNew();

                    Principal acPrincipal = null;

                    switch (lmsProvider.ToLower())
                    {
                        case LmsProviderNames.Canvas:
                            
                            sw = Stopwatch.StartNew();

                            if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token) || CanvasApi.IsTokenExpired(lmsCompany.LmsDomain, lmsUser.Token))
                            {
                                this.StartOAuth2Authentication(provider, key, param);
                                return null;
                            }

                            sw.Stop();
                            trace.AppendFormat("CanvasApi.IsTokenExpired: time: {0}.\r\n", sw.Elapsed.ToString());
                            sw = Stopwatch.StartNew();

                            if (lmsCompany.AdminUser == null)
                            {
                                this.logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsCompany.Id);
                                this.ViewBag.Message = Resources.Messages.LtiNoLmsAdmin;
                                return this.View("~/Views/Lti/LtiError.cshtml");
                            }

                            sw.Stop();
                            trace.AppendFormat("lmsCompany.AdminUser == null: time: {0}.\r\n", sw.Elapsed.ToString());
                            sw = Stopwatch.StartNew();

                            acPrincipal = acUserService.GetOrCreatePrincipal(
                                adobeConnectProvider,
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.lis_person_name_given,
                                param.lis_person_name_family,
                                lmsCompany);

                            sw.Stop();
                            trace.AppendFormat("acUserService.GetOrCreatePrincipal: time: {0}.\r\n", sw.Elapsed.ToString());
                            sw = Stopwatch.StartNew();

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
                                this.ViewBag.Message = Resources.Messages.LtiNoLmsAdmin;
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

                                // TRICK: save lmsUser to session
                                SaveSessionUser(session, lmsUser);
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
                        throw new Core.WarningMessageException(Resources.Messages.LtiNoAcAccount);
                    }
                    
                    return this.RedirectToExtJs(session, lmsUser, key, trace);
                }

                logger.ErrorFormat("Invalid LTI request. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.Error = Resources.Messages.LtiInvalidRequest;
                return this.View("Error");
            }
            catch (Core.WarningMessageException ex)
            {
                logger.WarnFormat("[WarningMessageException] param:{0}.", JsonConvert.SerializeObject(param, Formatting.Indented));
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            finally
            {
                methodTime.Stop();
                var time = methodTime.Elapsed;
                if (time > TimeSpan.FromSeconds(int.Parse((string)Settings.Monitoring_MaxLoginTime)))
                {
                    var monitoringLog = IoC.Resolve<ILogger>("Monitoring");

                    monitoringLog.ErrorFormat("LoginWithProvider takes more than {0} seconds. Time: {1}. Details: {2}.",
                        Settings.Monitoring_MaxLoginTime.ToString(),
                        time.ToString(), trace.ToString());
                }
            }
        }
        
        [HttpPost]
        public virtual JsonResult SetDefaultRolesForNonParticipants(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                string error = null;
                List<LmsUserDTO> updatedUsers = this.usersSetup.SetDefaultRolesForNonParticipants(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param,
                    meetingId,
                    out error);

                //if (string.IsNullOrEmpty(error))
                    return Json(OperationResultWithData<List<LmsUserDTO>>.Success(error, updatedUsers));

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

        private string ValidateLmsLicense(ILmsLicense lmsCompany, LtiParamDTO param)
        {
            if (lmsCompany != null)
            {
                if (!string.IsNullOrWhiteSpace(lmsCompany.LmsDomain) && !lmsCompany.HasLmsDomain(param.lms_domain))
                {
                    logger.ErrorFormat("LTI integration is already set for different domain. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationDifferentDomain;
                }

                if (!lmsCompany.IsActive)
                {
                    logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationInactiveLmsLicense;
                }
                
                if (!CompanyModel.IsActive(lmsCompany.CompanyId))
                {
                    logger.ErrorFormat("Company doesn't have any active license. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationInactiveCompanyLicense;
                }
            }
            else
            {
                logger.ErrorFormat("Adobe Connect integration is not set up. param:{0}.", JsonConvert.SerializeObject(param, Formatting.Indented));
                return string.Format(Resources.Messages.LtiValidationNoSetup);
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
        
        private static string GetUserNameOrEmail(LtiParamDTO param)
        {
            return string.IsNullOrWhiteSpace(param.lms_user_login) ? param.lis_person_contact_email_primary : param.lms_user_login;
        }

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
                        returnUrl, Core.Utils.Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);

                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, providerKey);
                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;
                case LmsProviderNames.Brightspace:
                    UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Core.Utils.Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);

                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
                    break;

            }
        }

        private ActionResult AuthCallbackSave(string providerKey, string provider, string token, string userId, string username, string viewName)
        {
            LmsUser lmsUser = null;
            LmsUserSession session = GetSession(providerKey);
            var company = session.With(x => x.LmsCompany);
            var param = session.With(x => x.LtiSession).With(x => x.LtiParam);
            if (!string.IsNullOrEmpty(token))
            {
                string userName = username;
                if (string.IsNullOrWhiteSpace(username) && (provider.ToLower() == LmsProviderNames.Canvas) && (param.lms_user_login == "$Canvas.user.loginId"))
                {
                    logger.Warn("[Canvas Auth Issue]. lms_user_login == '$Canvas.user.loginId'");
                    LmsUserDTO user = CanvasApi.GetUser(company.LmsDomain, token, userId);
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
                                param.lis_person_name_family ?? param.lis_person_name_full?.Split('@').FirstOrDefault(), // canvas can return empty lis_person_name_family in case when user was created only with email, lis_person_name_full is filled
                                company);
                if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
                {
                    lmsUser.PrincipalId = acPrincipal.PrincipalId;
                }

                this.lmsUserModel.RegisterSave(lmsUser);

                if (acPrincipal == null)
                {
                    this.logger.ErrorFormat("[AuthCallbackSave] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.", company.Id, lmsUser.Id, param.lms_user_login);
                    throw new Core.WarningMessageException(Resources.Messages.LtiNoAcAccount);
                }
            }

            if (company != null)
            {
                if (company.AdminUser == null)//this.IsAdminRole(providerKey))
                {
                    bool currentUserIsAdmin = IsAdminRole(param);
                    if (!currentUserIsAdmin && provider.ToLower() == LmsProviderNames.Brightspace) // todo: review. providerKey is guid
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
                                        (string) Settings.BrightspaceApiVersion, param.context_id) +
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

                    if (currentUserIsAdmin)
                    {
                        company.AdminUser = lmsUser;
                        lmsCompanyModel.RegisterSave(company);
                    }
                    else
                    {
                        this.logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", company.Id);
                        throw new Core.WarningMessageException(Resources.Messages.LtiNoLmsAdmin);
                    }
                }
                
                return this.RedirectToExtJs(session, lmsUser, providerKey);
            }

            this.ViewBag.Error = string.Format("Credentials not found");
            return View(viewName);
        }

        private ActionResult RedirectToExtJs(LmsUserSession session, LmsUser lmsUser, string providerName, StringBuilder trace = null)
        {
            var request = System.Web.HttpContext.Current.Request;

            FormCollection form = new FormCollection(request.Unvalidated().Form);

            var action = form["lti_action"];
            var ltiId = form["lti_id"];
            var eventId = form["sakai_id"];
            string tab = null;
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "join")
                {
                    int meetingId;
                    if (int.TryParse(ltiId, out meetingId))
                    {
                        return JoinMeeting(session.Id.ToString("n"), meetingId);
                    }
                }
                else if (action == "edit" || action == "delete")
                {
                    tab = "calendar";
                }
            }

            LtiViewModelDto model = BuildModel(session, trace);
            TempData["lti-index-model"] = model;

            var primaryColor = session.LmsUser.PrimaryColor;
            primaryColor = !string.IsNullOrWhiteSpace(primaryColor) ? primaryColor : (session.LmsCompany.PrimaryColor ?? string.Empty);

            return RedirectToAction("GetExtJsPage", "Lti", new
            {
                primaryColor = primaryColor,
                lmsProviderName = providerName,
                acConnectionMode = (int)lmsUser.AcConnectionMode,
                disableCacheBuster = true,
                tab = tab,
                meetingId = ltiId,
                eventId = eventId,
                eventAction = action
            });
        }

        private LtiViewModelDto BuildModel(LmsUserSession session, StringBuilder trace = null)
        {
            var sw = Stopwatch.StartNew();
            
            var credentials = session.LmsCompany;            
            var param = session.LtiSession.LtiParam;
            var acProvider = this.GetAdobeConnectProvider(credentials);

            sw.Stop();
            if (trace != null)
                trace.AppendFormat("GetAdobeConnectProvider: time: {0}.\r\n", sw.Elapsed.ToString());
            sw = Stopwatch.StartNew();

            var meetings = meetingSetup.GetMeetings(
                credentials,
                session.LmsUser,
                acProvider,
                param,
                trace);

            sw.Stop();
            if (trace != null)
                trace.AppendFormat("GetMeetings SUMMARY: time: {0}.\r\n", sw.Elapsed.ToString());

            sw = Stopwatch.StartNew();
            var acSettings = IoC.Resolve<API.AdobeConnect.IAdobeConnectAccountService>().GetAccountDetails(acProvider, _cache);
            sw.Stop();
            if (trace != null)
                trace.AppendFormat("AC - GetPasswordPolicies: time: {0}.\r\n", sw.Elapsed.ToString());

            IEnumerable<SeminarLicenseDto> seminars = null;
            string seminarsMessage = null;
            if (credentials.GetSetting<bool>(LmsCompanySettingNames.SeminarsEnable))
            {
                sw = Stopwatch.StartNew();

                IEnumerable<LmsCourseMeeting> seminarRecords = this.LmsCourseMeetingModel.GetSeminarsByCourseId(credentials.Id, param.course_id);

                try
                {
                    seminars = IoC.Resolve<API.AdobeConnect.ISeminarService>().GetLicensesWithContent(acProvider,
                        seminarRecords,
                        session.LmsUser, session.LtiSession.LtiParam, session.LmsCompany, acSettings.TimeZoneInfo);
                }
                catch (InvalidOperationException ex)
                {
                    // NOTE: a little bit tricky to catch InvalidOperationException
                    logger.Error("BuildModel.GetLicensesWithContent", ex);
                    seminarsMessage = "Seminars are not enabled for admin user set in the license.";
                }

                sw.Stop();
                if (trace != null)
                    trace.AppendFormat("AC - GetSeminars: time: {0}.\r\n", sw.Elapsed.ToString());

            }

            //TRICK: we calc shift on serverside
            acSettings.SetTimezoneShift(null);

            string userFullName = param.lis_person_name_full;
            var settings = LicenceSettingsDto.Build(credentials, LanguageModel.GetById(credentials.LanguageId), _cache);

            string version = typeof(LtiController).Assembly.GetName().Version.ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            var lmsProvider = LmsProviderModel.GetById(credentials.LmsProviderId);
            return new LtiViewModelDto
            {
                LtiVersion = version,

                // TRICK:
                // BB contains: lis_person_name_full:" Blackboard  Administrator"
                CurrentUserName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline),
                AcSettings = acSettings,
                AcRoles = new AcRole[] { AcRole.Host, AcRole.Presenter, AcRole.Participant },
                LicenceSettings = settings,
                Meetings = meetings,
                Seminars = seminars,
                SeminarsMessage = seminarsMessage,

                IsTeacher = UsersSetup.IsTeacher(param),
                ConnectServer = credentials.AcServer + "/",

                CourseMeetingsEnabled = credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,
                StudyGroupsEnabled = param.is_course_study_group_enabled.HasValue ? param.is_course_study_group_enabled.Value : credentials.EnableStudyGroups.GetValueOrDefault(),
                SyncUsersCountLimit = Core.Utils.Constants.SyncUsersCountLimit,

                LmsProviderName = lmsProvider.LmsProviderName,
                UserGuideLink = !string.IsNullOrEmpty(lmsProvider.UserGuideFileUrl)
                    ? lmsProvider.UserGuideFileUrl
                    : string.Format("/content/lti-instructions/{0}.pdf", lmsProvider.ShortName),
            };
        }
        
        private bool IsAdminRole(LtiParamDTO param)
        {
            if (param == null)
            {
                return this.IsDebug;
            }

            return param.roles.Contains("Administrator");
        }
        
        private LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;
            
            if (session == null)
            {
                logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        private LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        private API.AdobeConnect.IAdobeConnectProxy GetAdobeConnectProvider(ILmsLicense lmsCompany, bool forceReCreate = false)
        {
            API.AdobeConnect.IAdobeConnectProxy provider = null;
            if (forceReCreate ||
                ((provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] as API.AdobeConnect.IAdobeConnectProxy) == null))
            {
                provider = AdobeConnectAccountService.GetProvider(lmsCompany);
                this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] = provider;
            }
            
            return provider;
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
            session.SessionData = JsonConvert.SerializeObject(sessionData);
            this.userSessionModel.RegisterSave(session, flush: true);

            return session;
        }
        
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

            if (ex is IUserMessageException)
                return ex.Message;

            return IsDebug
                ? Resources.Messages.ExceptionOccured + ex.ToString()
                : Resources.Messages.ExceptionMessage;
        }

        #endregion

    }

}