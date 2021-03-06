﻿using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using EdugameCloud.Core;
using EdugameCloud.Core.Business;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Constants;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.OAuth;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using EdugameCloud.Lti.OAuth;
using EdugameCloud.Lti.OAuth.Canvas;
using EdugameCloud.Lti.OAuth.Desire2Learn;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Meeting;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.Constants;
using LtiLibrary.Core.Common;
using Microsoft.Web.WebPages.OAuth;
using HttpScheme = EdugameCloud.Lti.Core.Constants.HttpScheme;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiController : BaseController
    {
        private const string ProviderKeyCookieName = "providerKey";

        #region Fields

        private readonly LmsCompanyModel lmsCompanyModel;
        private readonly LmsUserSessionModel userSessionModel;
        private readonly LmsUserModel lmsUserModel;
        private readonly MeetingSetup meetingSetup;
        private readonly UsersSetup usersSetup;
        private readonly IAdobeConnectUserService acUserService;

        #endregion

        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();

        private ICanvasAPI CanvasApi => IoC.Resolve<ICanvasAPI>();

        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();

        private CompanyModel CompanyModel => IoC.Resolve<CompanyModel>();

        private LmsProviderModel LmsProviderModel => IoC.Resolve<LmsProviderModel>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();
        private IBuildVersionProcessor VersionProcessor => IoC.Resolve<IBuildVersionProcessor>();
        private ICache PersistantCache => IoC.Resolve<ICache>(CachePolicies.Names.PersistantCache);

        #region Constructors and Destructors

        public LtiController(
            LmsCompanyModel lmsCompanyModel,
            LmsUserSessionModel userSessionModel,
            LmsUserModel lmsUserModel, 
            MeetingSetup meetingSetup, 
            ApplicationSettingsProvider settings, 
            UsersSetup usersSetup,
            IAdobeConnectUserService acUserService,
            IAdobeConnectAccountService acAccountService,
            ILogger logger,
            IJsonSerializer json,
            ICache cache) :base(userSessionModel, acAccountService, settings, logger, json, cache)
        {
            this.lmsCompanyModel = lmsCompanyModel;
            this.userSessionModel = userSessionModel;
            this.lmsUserModel = lmsUserModel;
            this.meetingSetup = meetingSetup;
            this.usersSetup = usersSetup;
            this.acUserService = acUserService;
        }

        #endregion
        
        #region Public Methods and Operators

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed. Suppression is OK here.")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> AuthenticationCallback(
            // ReSharper disable once InconsistentNaming
            string __provider__,
            // ReSharper disable once InconsistentNaming
            string __sid__ = null,
            string code = null,
            string state = null,
            string session = null)
        {
            try
            {
                if (string.IsNullOrEmpty(__provider__))
                {
                    Logger.Error("[AuthenticationCallback] __provider__ parameter value is null or empty");
                    ViewBag.Error = Resources.Messages.NoLmsInformation;
                    return View("Error");
                }
                __provider__ = FixExtraDataIssue(__provider__);
                if (string.IsNullOrEmpty(session))
                {
                    if (Request.Cookies.AllKeys.Contains(ProviderKeyCookieName))
                    {
                        session = Request.Cookies[ProviderKeyCookieName].Value;
                    }
                    else
                    {
                        Logger.Error("[AuthenticationCallback] providerKey parameter value is null and there is no cookie with such name");
                        this.ViewBag.Error = Resources.Messages.NoSessionInformation;
                        return this.View("Error");
                    }
                }
                session = FixExtraDataIssue(session);
                string provider = __provider__;
                LmsUserSession s = GetSession(session);
                var param = s.With(x => x.LtiSession).With(x => x.LtiParam);

                if (provider == LmsProviderNames.Brightspace)
                {
                    var d2lService = IoC.Resolve<IDesire2LearnApiService>();

                    string scheme = Request.UrlReferrer.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
                    string authority = Request.UrlReferrer.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
                    var hostUrl = authority.Replace(scheme, string.Empty);

                    string username = null;
                    var lmsLicense = s.With(x => x.LmsCompany);
                    var user = d2lService.GetApiObjects<WhoAmIUser>(Request.Url, hostUrl, string.Format(d2lService.WhoAmIUrlFormat, (string)Settings.BrightspaceApiVersion), lmsLicense.GetLMSSettings(Settings));
                    if (string.IsNullOrEmpty(user.UniqueName))
                    {
                        var userInfo = d2lService.GetApiObjects<UserData>(Request.Url, hostUrl,
                            string.Format(d2lService.GetUserUrlFormat, (string)Settings.BrightspaceApiVersion, user.Identifier), lmsLicense.GetLMSSettings(Settings));
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
                        Logger.ErrorFormat("[AuthenticationCallback] UserId:{0}, UserKey:{1}", userId, userKey);
                        ViewBag.Error = Resources.Messages.CanSaveToDb;
                        return View("Error");
                    }

                    return await AuthCallbackSave(session, provider, token, user.Identifier, username, "Error");
                }
                else
                {
                    try
                    {
                        AuthenticationResult result;
                        if (provider == LmsProviderNames.Canvas)
                        {
                            var oAuthSettings = OAuthWebSecurityWrapper.GetOAuthSettings(s.LmsCompany, (string)Settings.CanvasClientId, (string)Settings.CanvasClientSecret);
                            result = OAuthWebSecurityWrapper.VerifyLtiAuthentication(HttpContext, oAuthSettings);
                        }
                        else
                        {
                            result = OAuthWebSecurity.VerifyAuthentication();
                        }

                        if (result.IsSuccessful)
                        {
                            if (provider.ToLower() == LmsProviderNames.Canvas)
                            {
                                if (param.lms_user_login == "$Canvas.user.loginId")
                                    throw new InvalidOperationException("[Canvas Authentication Error]. Please login to Canvas.");
                            }
                            //ACLTI-2120
                            if(!result.ExtraData.ContainsKey("accesstoken"))
                            {
                                Logger.Warn($"[Canvas AuthCallback] {JsonSerializer.JsonSerialize(result)}");
                                throw new InvalidOperationException("Unexpected error happened during authorization.");
                            }

                            return await AuthCallbackSave(session, provider,
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
                        Logger.ErrorFormat(ex, "[AuthenticationCallback] Application exception. SessionKey:{0}, Message:{1}", session, ex.Message);
                        ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                        ViewBag.Message = ex.Message;
                        return View("~/Views/Lti/LtiError.cshtml");
                    }
                }

                return this.View("Error");
            }
            catch (Core.WarningMessageException ex)
            {
                Logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", session);
                ViewBag.Message = ex.Message;
                return View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", session);
                ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return View("~/Views/Lti/LtiError.cshtml");
            }
        }

        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> GetExtJsPage(string primaryColor, string session, int acConnectionMode, bool disableCacheBuster = true)
        {
            try
            {
                var model = TempData["lti-index-model"] as LtiViewModelDto;

                // TRICK: to change lang inside
                LmsUserSession s = GetReadOnlySession(session);

                if (model == null)
                {
                    model = await BuildModelAsync(s);
                }
                return View(s.LmsCompany.LmsProviderId == (int) LmsProviderEnum.Bridge
                        ? "Bridge"
                        : "Index",
                    model);
            }
            catch (Core.WarningMessageException ex)
            {
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual async Task<ActionResult> JoinMeeting(string session, int meetingId)
        {
            Logger.Info($"JoinMeeting started. Session {session}");
            LmsCompany credentials = null;
            try
            {
                var s = GetReadOnlySession(session);
                credentials = s.LmsCompany;
                var param = s.LtiSession.LtiParam;
                var res = await meetingSetup.JoinMeeting(credentials, param, meetingId, GetAdminProvider(credentials));
                return LoginToAC(res.meetingJoinUrl, res.breezeSession, credentials);
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
                Logger.ErrorFormat(ex, "JoinMeeting exception. Id:{0}. SessionID: {1}.", meetingId, session);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual ActionResult JoinMeetingMobile(string session)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var s = GetReadOnlySession(session);
                lmsCompany = s.LmsCompany;
                var param = s.LtiSession.LtiParam;
                var provider = GetAdminProvider(lmsCompany);
                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException(string.Format(Resources.Messages.NoUserFound, param.lms_user_id));
                }

                string breezeSession = meetingSetup.ACLogin(
                        lmsCompany,
                        param,
                        lmsUser,
                        provider).BreezeSession;
                
                if (string.IsNullOrWhiteSpace(breezeSession))
                    return Json(OperationResult.Error(Resources.Messages.CanNotGetBreezeSession), JsonRequestBehavior.AllowGet);

                return Json(breezeSession.ToSuccessResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinMeeting", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [ValidateInput(false)]
        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            Logger.Info($"LoginWithProvider started with key {param.oauth_consumer_key}, user is: {param.user_id ?? string.Empty}");
            var methodTime = Stopwatch.StartNew();
            var trace = new StringBuilder();
            try
            {
                // TRICK: to save course_id in DB;
                param.CalculateFields();

                // Parse and validate the request
                Request.CheckForRequiredLtiParameters();

                var sw = Stopwatch.StartNew();

                LmsCompany lmsCompany =
                    this.lmsCompanyModel.GetOneByConsumerKey(param.oauth_consumer_key, includeSettings:true)
                        .Value;

                if (lmsCompany != null)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo(LanguageModel.GetById(lmsCompany.LanguageId).TwoLetterCode);
                }
                else
                {
                    Logger.ErrorFormat("Adobe Connect integration is not set up. param:{0}.", JsonSerializer.JsonSerialize(param));
                    throw new LtiException($"{Resources.Messages.LtiInvalidRequest}. {Resources.Messages.LtiValidationNoSetup}");
                }

                SaveLMSDomainIfEmpty(param, lmsCompany); //setting domain before validation
                string validationError = ValidateLmsLicense(lmsCompany, param);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    this.ViewBag.Error = validationError;
                    return this.View("Error");
                }

                sw.Stop();
                trace.AppendFormat("GetOneByConsumerKey and ValidateLmsLicense: time: {0}.\r\n", sw.Elapsed.ToString());

                LmsProvider providerInstance = LmsProviderModel.GetOneById(lmsCompany.LmsProviderId).Value;
                string lmsProvider = providerInstance.ShortName;

                if (lmsProvider.ToLower() == LmsProviderNames.Brightspace && !string.IsNullOrEmpty(param.user_id))
                {
                    Logger.InfoFormat("[D2L login attempt]. Original user_id: {0}. oauth_consumer_key:{1}.",
                        param.user_id, param.oauth_consumer_key);
                    var parsedIdArray = param.user_id.Split('_');
                    // temporary fix
                    if (parsedIdArray.Length > 1)
                    {
                        param.user_id = parsedIdArray.Last();
                    }
                }

                if (lmsProvider.ToLower() == LmsProviderNames.Haiku)
                {
                    param.user_id = param.user_id.TrimEnd('_');
                }

                if (lmsProvider.ToLower() == LmsProviderNames.Canvas)
                {
                    if (Request.Headers.HasKey("Referer"))
                    {
                        param.referer = Request.Headers["Referer"].ToString();
                    }
                }

                sw = Stopwatch.StartNew();

                if (!BltiProviderHelper.VerifyBltiRequest(lmsCompany, Request,
                        () => true)) //todo: remove if not needed
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid signature. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                    throw new LtiException($"{Resources.Messages.LtiInvalidRequest}. {Resources.Messages.LtiValidationWrongSignature}");
                }

                sw.Stop();
                trace.AppendFormat("VerifyBltiRequest: time: {0}.\r\n", sw.Elapsed.ToString());

                ValidateLtiVersion(param);

                ValidateIntegrationRequiredParameters(lmsCompany, param);

                sw = Stopwatch.StartNew();
                var adobeConnectProvider = this.GetAdminProvider(lmsCompany);
                sw.Stop();
                trace.AppendFormat("GetAdobeConnectProvider: time: {0}.\r\n", sw.Elapsed.ToString());

                sw = Stopwatch.StartNew();

                // TRICK: if LMS don't return user login - try to call lms' API to fetch user's info using user's LMS-ID.
                param.ext_user_username = await usersSetup.GetParamLogin(param, lmsCompany); // NOTE: is saved in session!

                sw.Stop();
                trace.AppendFormat("GetParamLogin: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                sw.Stop();
                trace.AppendFormat("GetOneByUserIdAndCompanyLms: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                LmsUserSession session = this.SaveSession(lmsCompany, param, lmsUser);
                var sessionKey = session.Id.ToString();

                sw.Stop();
                trace.AppendFormat("SaveSession: time: {0}.\r\n", sw.Elapsed.ToString());
                sw = Stopwatch.StartNew();

                // NOTE: not in use - read during license save. this.meetingSetup.SetupFolders(lmsCompany, adobeConnectProvider);

                sw.Stop();
                trace.AppendFormat("meetingSetup.SetupFolders: time: {0}.\r\n", sw.Elapsed.ToString());
               
                Principal acPrincipal = null;

                switch (lmsProvider.ToLower())
                {
                    case LmsProviderNames.Canvas:

                        sw = Stopwatch.StartNew();

                        if (string.IsNullOrWhiteSpace(lmsUser?.Token) ||
                            await CanvasApi.IsTokenExpired(lmsCompany.LmsDomain, lmsUser.Token))
                        {
                            this.StartOAuth2Authentication(lmsCompany, lmsProvider, sessionKey, param);
                            return null;
                        }

                        sw.Stop();
                        trace.AppendFormat("CanvasApi.IsTokenExpired: time: {0}.\r\n", sw.Elapsed.ToString());
                        sw = Stopwatch.StartNew();

                        if (lmsCompany.AdminUser == null)
                        {
                            Logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsCompany.Id);
                            this.ViewBag.Message = Resources.Messages.LtiNoLmsAdmin;
                            return this.View("~/Views/Lti/LtiError.cshtml");
                        }

                        sw.Stop();
                        trace.AppendFormat("lmsCompany.AdminUser == null: time: {0}.\r\n", sw.Elapsed.ToString());
                        sw = Stopwatch.StartNew();

                        ///Old logic for Canvas LMS: FirstName and LastName were gotten from SortableName Property.
                        ///(LTI parameter: lis_person_name_given)
                        ///Now according to task https://jira.esynctraining.com/browse/ACLTI-2137
                        ///We get fistName and LastName from FullName property (LTI param: lis_person_name_full).
                        acPrincipal = acUserService.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.FirstNameFromFullNameParam,
                            param.LastNameFromFullNameParam,
                            lmsCompany);


                        sw.Stop();
                        trace.AppendFormat("acUserService.GetOrCreatePrincipal: time: {0}.\r\n",
                            sw.Elapsed.ToString());

                        break;
                    case LmsProviderNames.Brightspace:
                        //todo: review. Probably we need to redirect to auth url everytime for overwriting tokens if user logs in under different roles
                        if (lmsUser == null || string.IsNullOrWhiteSpace(lmsUser.Token))
                        {
                            string schema = Request.GetScheme();

                            var d2lService = IoC.Resolve<IDesire2LearnApiService>();
                            string returnUrl = Url.AbsoluteCallbackAction(schema, new { __provider__ = lmsProvider });

                            Response.Cookies.Add(new HttpCookie(ProviderKeyCookieName, sessionKey));
                            return Redirect(
                                d2lService
                                    .GetTokenRedirectUrl(new Uri(returnUrl), param.lms_domain, lmsCompany.GetLMSSettings(Settings))
                                    .AbsoluteUri);
                        }

                        if (lmsCompany.AdminUser == null)
                        {
                            Logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsCompany.Id);
                            this.ViewBag.Message = Resources.Messages.LtiNoLmsAdmin;
                            return this.View("~/Views/Lti/LtiError.cshtml");
                        }

                        acPrincipal = acUserService.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.PersonNameGiven,
                            param.PersonNameFamily,
                            lmsCompany);
                        break;
                    case LmsProviderNames.AgilixBuzz:
                    case LmsProviderNames.Blackboard:
                    case LmsProviderNames.Moodle:
                    case LmsProviderNames.Sakai:
                    case LmsProviderNames.Schoology:
                    case LmsProviderNames.Haiku:
                    case LmsProviderNames.Bridge:
                        //                    case LmsProviderNames.IMS:
                        acPrincipal = acUserService.GetOrCreatePrincipal(
                            adobeConnectProvider,
                            param.lms_user_login,
                            param.lis_person_contact_email_primary,
                            param.PersonNameGiven,
                            param.PersonNameFamily,
                            lmsCompany);

                        if (lmsUser == null)
                        {
                            lmsUser = new LmsUser
                            {
                                LmsCompany = lmsCompany,
                                UserId = param.lms_user_id,
                                Username = param.GetUserNameOrEmail(),
                                PrincipalId = acPrincipal?.PrincipalId,
                                Name = param.lis_person_name_full,
                                Email = param.lis_person_contact_email_primary
                            };
                            this.lmsUserModel.RegisterSave(lmsUser);

                            // TRICK: save lmsUser to session
                            SaveSessionUser(session, lmsUser);
                        }


                        // TO SOLVE PROBLEM That existed user was created without NAME.
                        if (string.IsNullOrEmpty(lmsUser.Name))
                        {
                            lmsUser.Name = param.lis_person_name_full;
                            this.lmsUserModel.RegisterSave(lmsUser);
                        }

                        // TO SOLVE PROBLEM That existed user was created without Email
                        if (string.IsNullOrEmpty(lmsUser.Email))
                        {
                            lmsUser.Email = param.lis_person_contact_email_primary;
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
                    Logger.ErrorFormat(
                        "[LoginWithProvider] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.",
                        lmsCompany.Id, lmsUser.Id, param.lms_user_login);
                    throw new Core.WarningMessageException(Resources.Messages.LtiNoAcAccount);
                }

                return await RedirectToExtJs(session, lmsUser, trace);
            }
            catch (LtiException ex)
            {
                Logger.Error("Lti exception", ex);
                ViewBag.Message = $"Invalid LTI request. {ex.Message}";
                if (!string.IsNullOrEmpty(param.launch_presentation_return_url))
                {
                    ViewBag.ReturnUrl = param.launch_presentation_return_url;
                }
                return View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Core.WarningMessageException ex)
            {
                Logger.WarnFormat("[WarningMessageException] param:{0}.",
                    JsonSerializer.JsonSerialize(param));
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.", param.oauth_consumer_key);
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
        
        #endregion

        #region Methods

        private string ValidateLmsLicense(ILmsLicense lmsLicense, LtiParamDTO param)
        {
//            if (lmsLicense != null)
//            {
                if (!lmsLicense.HasLmsDomain(param.lms_domain))
                {
                    Logger.ErrorFormat("LTI integration is already set for different domain. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationDifferentDomain;
                }

                if (!lmsLicense.IsActive)
                {
                    Logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationInactiveLmsLicense;
                }
                
                if (!CompanyModel.IsActive(lmsLicense.CompanyId))
                {
                    Logger.ErrorFormat("Company doesn't have any active license. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                    return Resources.Messages.LtiValidationInactiveCompanyLicense;
                }
//            }
//            else
//            {
//                logger.ErrorFormat("Adobe Connect integration is not set up. param:{0}.", JsonUtility.JsonSerialize(param));
//                return string.Format(Resources.Messages.LtiValidationNoSetup);
//            }

            return null;
        }

        private void ValidateLtiVersion(LtiParamDTO param)
        {
            // in case when client supports v2.0 - just warn, for our AC integration all necessary functionality should be supported
            if (param.lti_version == "")
            {
                Logger.Warn($"[LtiVersion - 2.0] ConsumerKey={param.oauth_consumer_key}");
            }
            //version should match "LTI-1p0" for v1.0, v1.1, v1.2
            else if (param.lti_version != LtiConstants.LtiVersion && param.lti_version != "LTI-1p2") //bridge uses 1p2, todo: search for correct validation
            {
                Logger.ErrorFormat("Invalid LTI request. Invalid LTI version. oauth_consumer_key:{0}, lti_version:{1}", param.oauth_consumer_key, param.lti_version);
                throw new LtiException(Resources.Messages.LtiValidationWrongVersion);
            }
        }

        private void ValidateIntegrationRequiredParameters(LmsCompany lmsCompany, LtiParamDTO param)
        {
            var missingIntegrationRequiredFields = new HashSet<string>();
            if (string.IsNullOrEmpty(param.context_id))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.CourseId);
            if (string.IsNullOrEmpty(param.user_id))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.UserId);
            if (string.IsNullOrEmpty(param.PersonNameGiven))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.FirstName);
            if (string.IsNullOrEmpty(param.PersonNameFamily))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.LastName);
            if (lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault() && string.IsNullOrEmpty(param.lis_person_contact_email_primary))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.Email);

            if (missingIntegrationRequiredFields.Any())
            {
                throw new LtiException($"{Resources.Messages.LtiValidationRequiredACParameters} {string.Join(", ", missingIntegrationRequiredFields.ToArray())}");
            }
        }
        
        private static string FixExtraDataIssue(string keyToFix)
        {
            if (keyToFix != null && keyToFix.Contains(","))
            {
                var keys = keyToFix.Split(",".ToCharArray());
                keyToFix = keys.FirstOrDefault().Return(x => x, keyToFix);
            }

            return keyToFix;
        }
        
        private void SaveLMSDomainIfEmpty(LtiParamDTO param, LmsCompany lmsLicense)
        {
            if (string.IsNullOrWhiteSpace(lmsLicense.LmsDomain))
            {
                // NOTE: e.g. we do not setup domain for Canvas license
                lmsLicense.LmsDomain = param.lms_domain;
                lmsCompanyModel.RegisterSave(lmsLicense, true);
            }
        }

        private void StartOAuth2Authentication(LmsCompany lmsCompany, string provider, string session, LtiParamDTO model)
        {
            string schema = Request.GetScheme();

            string returnUrl = this.Url.AbsoluteCallbackAction(schema, new { __provider__ = provider, session });
            switch (provider)
            {
                case LmsProviderNames.Canvas:
                    returnUrl = UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Core.Utils.Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);

                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, session);
                    var oAuthSettings = OAuthWebSecurityWrapper.GetOAuthSettings(lmsCompany, (string)Settings.CanvasClientId, (string)Settings.CanvasClientSecret);
                    if (string.IsNullOrEmpty(oAuthSettings.Key) || string.IsNullOrEmpty(oAuthSettings.Value))
                    {
                        var message = Resources.Messages.LtiOauthInvalidParameters;
                        throw new LtiException(message);
                    }
                    OAuthWebSecurityWrapper.RequestAuthentication(HttpContext, oAuthSettings, returnUrl);
                    break;
                    // not used
//                case LmsProviderNames.Brightspace:
//                    UriBuilderExtensions.AddQueryStringParameter(
//                        returnUrl, Core.Utils.Constants.ReturnUriExtensionQueryParameterName, HttpScheme.Https + model.lms_domain);
//
//                    OAuthWebSecurity.RequestAuthentication(provider, returnUrl);
//                    break;

            }
        }

        private async Task<ActionResult> AuthCallbackSave(string providerKey, string provider, string token, string userId, string username, string viewName)
        {
            LmsUser lmsUser = null;
            LmsUserSession session = GetSession(providerKey);
            var lmsLicense = session.With(x => x.LmsCompany);
            var param = session.With(x => x.LtiSession).With(x => x.LtiParam);
            if (!string.IsNullOrEmpty(token))
            {
                string userName = username;
                if (string.IsNullOrWhiteSpace(username) && (provider.ToLower() == LmsProviderNames.Canvas) && (param.lms_user_login == "$Canvas.user.loginId"))
                {
                    Logger.Warn("[Canvas Auth Issue]. lms_user_login == '$Canvas.user.loginId'");
                    LmsUserDTO user = await CanvasApi.GetUser(lmsLicense.LmsDomain, token, userId);
                    if (user != null)
                        userName = user.Login;
                }

                if (string.IsNullOrWhiteSpace(username))
                    userName = param.GetUserNameOrEmail();

                lmsUser = this.lmsUserModel.GetOneByUserIdAndCompanyLms(userId, lmsLicense.Id).Value 
                    ?? new LmsUser {
                                    UserId = userId,
                                    LmsCompany = lmsLicense,
                                    Username = userName,
                                    Name = param.lis_person_name_full,
                                    Email = param.lis_person_contact_email_primary
                                    };

                lmsUser.Username = userName;
                lmsUser.Token = token;
                
                // TRICK: during loginwithprovider we redirect to Oauth before we create AC principal - so we need to do it here
                Principal acPrincipal = acUserService.GetOrCreatePrincipal(
                                this.GetAdminProvider(lmsLicense),
                                param.lms_user_login,
                                param.lis_person_contact_email_primary,
                                param.PersonNameGiven,
                                param.PersonNameFamily,
                                lmsLicense);
                if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
                {
                    lmsUser.PrincipalId = acPrincipal.PrincipalId;
                }

                // TRICK: call it if U R sure that GetOrCreatePrincipal will not fail!!
                // NHibernate error could occur instead
                bool isTransientUser = lmsUser.IsTransient();
                this.lmsUserModel.RegisterSave(lmsUser);
                if (isTransientUser)
                {
                    //SAVE current lms user
                    this.SaveSessionUser(session, lmsUser);
                }

                if (acPrincipal == null)
                {
                    Logger.ErrorFormat("[AuthCallbackSave] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.", lmsLicense.Id, lmsUser.Id, param.lms_user_login);
                    throw new Core.WarningMessageException(Resources.Messages.LtiNoAcAccount);
                }
            }

            if (lmsLicense != null)
            {
                if (lmsLicense.AdminUser == null)//this.IsAdminRole(providerKey))
                {
                    bool currentUserIsAdmin = IsAdminRole(param);
                    if (!currentUserIsAdmin && provider.ToLower() == LmsProviderNames.Brightspace)
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
                                    lmsLicense.GetLMSSettings(Settings));
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
                        //SAVE license admin user
                        lmsLicense.AdminUser = lmsUser;
                        lmsCompanyModel.RegisterSave(lmsLicense);
                    }
                    else
                    {
                        Logger.ErrorFormat("LMS Admin is not set. LmsCompany ID: {0}.", lmsLicense.Id);
                        throw new Core.WarningMessageException(Resources.Messages.LtiNoLmsAdmin);
                    }
                }
                
                return await RedirectToExtJs(session, lmsUser);
            }

            this.ViewBag.Error = string.Format("Credentials not found");
            return View(viewName);
        }

        private async Task<ActionResult> RedirectToExtJs(LmsUserSession session, LmsUser lmsUser, StringBuilder trace = null)
        {
            var request = Request;
            var form = new FormCollection(request.Unvalidated().Form);

            var action = form["lti_action"];
            var ltiId = form["lti_id"];
            var sakaiId = form["sakai_id"];
            int? eventId = null;
            int meetingId;
            string tab = null;
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "join")
                {
                    if (int.TryParse(ltiId, out meetingId))
                    {
                        return await JoinMeeting(session.Id.ToString("n"), meetingId);
                    }
                }
                else if (action == "edit" || action == "delete")
                {
                    tab = "calendar";
                }
            }

            LtiViewModelDto model = await BuildModelAsync(session, trace);
            TempData["lti-index-model"] = model;

            var primaryColor = session.LmsCompany.PrimaryColor ?? string.Empty;

            if (!string.IsNullOrEmpty(sakaiId) && int.TryParse(ltiId, out meetingId))
            {
                var meeting = LmsCourseMeetingModel.GetOneById(meetingId).Value;
                var meetingSession = meeting?.MeetingSessions.SingleOrDefault(x => x.LmsCalendarEventId == sakaiId);
                if (meetingSession != null)
                {
                    eventId = meetingSession.Id;
                }
            }

            return RedirectToAction("GetExtJsPage", "Lti", new
            {
                primaryColor = primaryColor,
                session = session.Id.ToString(),
                acConnectionMode = (int)lmsUser.AcConnectionMode,
                disableCacheBuster = true,
                tab = tab,
                meetingId = ltiId,
                eventId = eventId,
                eventAction = action
            });
        }

        private async Task<LtiViewModelDto> BuildModelAsync(LmsUserSession session, StringBuilder trace = null)
        {
            var sw = Stopwatch.StartNew();
            
            var credentials = session.LmsCompany;
            var param = session.LtiSession.LtiParam;
            var acProvider = this.GetAdminProvider(credentials);

            sw.Stop();
            trace?.AppendFormat("GetAdobeConnectProvider: time: {0}.\r\n", sw.Elapsed.ToString());
            sw = Stopwatch.StartNew();

            var meetings = await meetingSetup.GetMeetingsAsync(
                credentials,
                acProvider,
                session.LmsUser,
                param,
                trace);

            sw.Stop();
            trace?.AppendFormat("GetMeetings SUMMARY: time: {0}.\r\n", sw.Elapsed.ToString());

            sw = Stopwatch.StartNew();
            var acSettings = IoC.Resolve<API.AdobeConnect.IAdobeConnectAccountService>().GetAccountDetails(acProvider, Cache);
            sw.Stop();
            trace?.AppendFormat("AC - GetPasswordPolicies: time: {0}.\r\n", sw.Elapsed.ToString());

            IEnumerable<SeminarLicenseDto> seminars = null;
            string seminarsMessage = null;
            if (credentials.GetSetting<bool>(LmsLicenseSettingNames.SeminarsEnable))
            {
                sw = Stopwatch.StartNew();

                IEnumerable<LmsCourseMeeting> seminarRecords = this.LmsCourseMeetingModel.GetSeminarsByCourseId(credentials.Id, param.course_id);

                try
                {
                    seminars = IoC.Resolve<API.AdobeConnect.ISeminarService>().GetLicensesWithContent(acProvider,
                        seminarRecords,
                        session.LmsUser, session.LtiSession.LtiParam, session.LmsCompany/*, acSettings.TimeZoneInfo*/);
                }
                catch (InvalidOperationException ex)
                {
                    // NOTE: a little bit tricky to catch InvalidOperationException
                    Logger.Error("BuildModel.GetLicensesWithContent", ex);
                    seminarsMessage = "Seminars are not enabled for admin user set in the license.";
                }

                sw.Stop();
                trace?.AppendFormat("AC - GetSeminars: time: {0}.\r\n", sw.Elapsed.ToString());
            }

            //TRICK: we calc shift on serverside
            //acSettings.SetTimezoneShift(null);

            string userFullName = param.lis_person_name_full ?? param.lis_person_name_given + " " + param.lis_person_name_family;
            var settings = LicenseSettingsDto.Build(credentials, LanguageModel.GetById(credentials.LanguageId), Cache);

            var filePattern = (string)Settings.JsBuildSelector;
            var versionFileJs = CacheUtility.GetCachedItem<Version>(PersistantCache, CachePolicies.Keys.VersionFileName(filePattern), () =>
            {
                return VersionProcessor.ProcessVersion(Server.MapPath("~/extjs/"), filePattern);
            });

            LtiViewModelDto.SettingsInfo.ActionUrls.RestWebApiBaseUrl = (string)Settings.LtiRestWebApiBaseUrl;

            var lmsProvider = LmsProviderModel.GetById(credentials.LmsProviderId);
            var model = new LtiViewModelDto
            {
                FullVersion = versionFileJs,
//                LtiVersion = version,

                // TRICK:
                // BB contains: lis_person_name_full:" Blackboard  Administrator"
                CurrentUserName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline),
                AcSettings = acSettings,
                AcRoles = new AcRole[] { AcRole.Host, AcRole.Presenter, AcRole.Participant },
                LicenseSettings = settings,
                Meetings = meetings,
                Seminars = seminars,
                SeminarsMessage = seminarsMessage,

                IsTeacher = UsersSetup.IsTeacher(param, credentials),
                ConnectServer = credentials.AcServer + "/",

                CourseMeetingsEnabled = credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,
                StudyGroupsEnabled = param.is_course_study_group_enabled.HasValue ? param.is_course_study_group_enabled.Value : credentials.EnableStudyGroups.GetValueOrDefault(),
                SyncUsersCountLimit = Core.Utils.Constants.SyncUsersCountLimit,

                LmsProviderName = lmsProvider.LmsProviderName,
                UserGuideLink = !string.IsNullOrEmpty(lmsProvider.UserGuideFileUrl)
                    ? lmsProvider.UserGuideFileUrl
                    : new Uri(new Uri((string)Settings.BasePath, UriKind.Absolute), $"content/lti-instructions/{lmsProvider.LmsProviderName}.pdf").ToString(),
            };

            bool hasMp4 = settings.HasMp4ServiceLicenseKey | settings.HasMp4ServiceWithSubtitlesLicenseKey;
            if (hasMp4)
                model.ActionUrls.GetRecordings = model.ActionUrls.GetMP4Recordings;

            return model;
        }
        
        private bool IsAdminRole(LtiParamDTO param)
        {
            if (param == null)
            {
                return this.IsDebug;
            }

            return param.roles.Contains("Administrator");
        }
        
        private LmsUserSession SaveSession(LmsCompany company, LtiParamDTO param, LmsUser lmsUser)
        {
            var session = (lmsUser == null) ? null : this.userSessionModel.GetOneByCompanyAndUserAndCourse(lmsUser.Id, param.course_id).Value;
            session = session ?? new LmsUserSession { LmsCompany = company, LmsUser = lmsUser, LmsCourseId = param.course_id };
            var sessionData = new LtiSessionDTO { LtiParam = param };
            session.SessionData = JsonSerializer.JsonSerialize(sessionData);
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
        
        #endregion

    }

}