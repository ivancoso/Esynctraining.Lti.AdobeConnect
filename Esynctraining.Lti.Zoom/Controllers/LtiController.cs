using Esynctraining.Core.Json;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.API.Desire2Learn;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Constants;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Lti.Zoom.Domain.Entities;
using Esynctraining.Lti.Zoom.DTO;
using Esynctraining.Lti.Zoom.Extensions;
using Esynctraining.Lti.Zoom.OAuth;
using Esynctraining.Zoom.ApiWrapper;
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpScheme = Esynctraining.Lti.Zoom.Constants.HttpScheme;
using ILogger = Esynctraining.Core.Logging.ILogger;
using OAuthTokenResponse = Esynctraining.Lti.Lms.Common.API.Canvas.OAuthTokenResponse;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public partial class LtiController : BaseController
    {
        private const string ProviderKeyCookieName = "providerKey";

        private readonly ILmsLicenseService _licenseService;
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _userService;
        private readonly LmsUserServiceFactory _lmsUserServiceFactory;
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly IEGCEnabledCanvasAPI _canvasApi;
        private readonly IDesire2LearnApiService _brightSpaceApi;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public LtiController(ILogger logger, ApplicationSettingsProvider settings, ILmsLicenseService licenseService,
            UserSessionService sessionService, IJsonDeserializer jsonDeserializer, ZoomMeetingService meetingService,
            ZoomUserService userService, LmsUserServiceFactory lmsUserServiceFactory,
            ILmsLicenseService lmsLicenseService, IEGCEnabledCanvasAPI canvasApi, 
            IDesire2LearnApiService brightSpaceApi, ILogger<LtiController> msLogger) : base(logger, settings, sessionService)
        {
            _licenseService = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
            _jsonDeserializer = jsonDeserializer ?? throw new ArgumentNullException(nameof(meetingService));
            _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _lmsUserServiceFactory = lmsUserServiceFactory ?? throw new ArgumentNullException(nameof(lmsUserServiceFactory));
            _lmsLicenseService = lmsLicenseService ?? throw new ArgumentNullException(nameof(lmsLicenseService));
            _canvasApi = canvasApi ?? throw new ArgumentNullException(nameof(canvasApi));
            _brightSpaceApi = brightSpaceApi ?? throw new ArgumentNullException(nameof(brightSpaceApi));
            _logger = msLogger ?? throw new ArgumentNullException(nameof(msLogger));
        }

        [HttpGet]
        public virtual async Task<ActionResult> JoinMeeting(int meetingId, string session)
        {
            var userSession = await GetSession(session);
            var license = await _licenseService.GetLicense(userSession.LicenseKey);
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(userSession.SessionData);
            var dbMeeting = await _meetingService.GetMeeting(meetingId, param.course_id.ToString());

            if (dbMeeting == null)
                return NotFound(meetingId);
            UserInfoDto zoomUser = null;
            //string userId;
            try
            {
                zoomUser = await _userService.GetUser(param.lis_person_contact_email_primary);
                if (zoomUser.Status != ZoomUserStatus.Active)
                {
                    ViewBag.Message =
                        "Your account is either in 'pending' or 'inactive' status. Check email for registration link or contact your Zoom account manager.";
                    return this.View("~/Views/Lti/LtiError.cshtml");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"User {param.lis_person_contact_email_primary} doesn't exist or doesn't belong to zoom account of license {param.oauth_consumer_key}", e);

                try
                {
                    var userInfo = await _userService.CreateUser(new CreateUserDto
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.PersonNameGiven,
                        LastName = param.PersonNameFamily
                    });
                }
                catch (ZoomApiException ex)
                {
                    Logger.Error(
                        $"[ZoomApiException] Status:{ex.StatusDescription}, Content:{ex.Content}, ErrorMessage: {ex.ErrorMessage}",
                        ex);
                }

                ViewBag.Message =
                    "The invitation to Zoom account was sent to your email. Please check your email and try to log in to LMS again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            var url = await _meetingService.GetMeetingUrl(zoomUser.Id, dbMeeting.ProviderMeetingId,
                param.lis_person_contact_email_primary,
                async () =>
                {
                    var lmsSettings = license.GetLMSSettings(userSession);
                    var lmsService = _lmsUserServiceFactory.GetUserService(license.ProductId);
                    var lmsUserExistsInCourse = await lmsService.GetUser(lmsSettings, userSession.LmsUserId, userSession.CourseId);
                    if (!lmsUserExistsInCourse.IsSuccess)
                    {
                        Logger.Warn(
                            $"[JoinMeeting:{meetingId}] LmsUserId:{userSession.LmsUserId}, Message:{lmsUserExistsInCourse.Message}");
                        return null;
                    }

                    return
                        new RegistrantDto
                        {
                            Email = param.lis_person_contact_email_primary,
                            FirstName = param.PersonNameGiven,
                            LastName = param.PersonNameFamily
                        };
                });
            return Redirect(url);
        }

        [HttpGet]
        public virtual async Task<ActionResult<JoinLinkParamDto>> JoinMeetingMobile(int meetingId, string session)
        {
            var userSession = await GetSession(session);
            //var license = await _licenseService.GetLicense(userSession.LicenseKey);
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(userSession.SessionData);
            var dbMeeting = await _meetingService.GetMeeting(meetingId, param.course_id.ToString());

            if (dbMeeting == null)
                return NotFound(meetingId);
            UserInfoDto zoomUser = null;
            //string userId;
            try
            {
                zoomUser = await _userService.GetUser(param.lis_person_contact_email_primary);
            }
            catch (Exception e)
            {
                Logger.Error($"User {param.lis_person_contact_email_primary} doesn't exist or doesn't belong to this account", e);

                var userInfo = await _userService.CreateUser(new CreateUserDto
                {
                    Email = param.lis_person_contact_email_primary,
                    FirstName = param.PersonNameGiven,
                    LastName = param.PersonNameFamily
                });

                return Content(
                    "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
            }

            var joinLinkParam = new JoinLinkParamDto()
            {
                ConfoNo = dbMeeting.ProviderMeetingId,
                Uid = zoomUser.Id,
                Uname = $"{zoomUser.FirstName} {zoomUser.LastName}",
                Tk = await _meetingService.GetToken(zoomUser.Id, "token"),
                Zpk = await _meetingService.GetToken(zoomUser.Id, "zpk"),
                Zak = await _meetingService.GetToken(zoomUser.Id, "zak"),
                Email = param.lis_person_contact_email_primary
            };

            return joinLinkParam;
        }

        public async Task<ActionResult> Home(string session)
        {
            try
            {
                LmsUserSession s = await _sessionService.GetSession(Guid.Parse(session));
                var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);
                try
                {
                    var zoomUser = await _userService.GetUser(param.lis_person_contact_email_primary);
                    if (zoomUser.Status != ZoomUserStatus.Active)
                    {
                        ViewBag.Message =
                            "Your account is either in 'pending' or 'inactive' status. Check email for registration link or contact your Zoom account manager.";
                        return this.View("~/Views/Lti/LtiError.cshtml");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"User {param.lis_person_contact_email_primary} doesn't exist or doesn't belong to zoom account of license {param.oauth_consumer_key}", e);

                    try
                    {
                        var userInfo = await _userService.CreateUser(new CreateUserDto
                        {
                            Email = param.lis_person_contact_email_primary,
                            FirstName = param.PersonNameGiven,
                            LastName = param.PersonNameFamily
                        });
                    }
                    catch (ZoomApiException ex)
                    {
                        Logger.Error(
                            $"[ZoomApiException] Status:{ex.StatusDescription}, Content:{ex.Content}, ErrorMessage: {ex.ErrorMessage}",
                            ex);
                    }

                    ViewBag.Message =
                        "The invitation to Zoom account was sent to your email. Please check your email and try to log in to LMS again.";
                    return this.View("~/Views/Lti/LtiError.cshtml");
                }
                
                var model = await BuildModelAsync(s);
                return View("Index", model);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error with Session :{session}", ex);
                ViewBag.Message = "Unexpected error occurred. Please contact support.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification =
            "Reviewed. Suppression is OK here.")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> AuthenticationCallback(
            string __provider__,
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
                    ViewBag.Error = "Could not find LMS information. Please, contact system administrator.";
                    return View("~/Views/Lti/LtiError.cshtml");
                }
                __provider__ = FixExtraDataIssue(__provider__);
                if (string.IsNullOrEmpty(session))
                {
                    if (Request.Cookies.Keys.Contains(ProviderKeyCookieName))
                    {
                        session = Request.Cookies[ProviderKeyCookieName];
                    }
                    else
                    {
                        Logger.Error(
                            "[AuthenticationCallback] providerKey parameter value is null and there is no cookie with such name");
                        this.ViewBag.Error =
                            "Could not find session information for current user. Please, enable cookies or try to open LTI application in a different browser.";
                        return this.View("~/Views/Lti/LtiError.cshtml");
                    }
                }
                session = FixExtraDataIssue(session);
                string provider = __provider__;
                LmsUserSession s = await GetSession(session);

                var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);

                try
                {
                    if (provider == LmsProviderNames.Canvas)
                    {

                        var license = await _licenseService.GetLicense(Guid.Parse(param.oauth_consumer_key));
                        var oAuthId = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                        var oAuthKey = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthKey);
                        var settingsBasePath = (string)Settings.BasePath;
                        var basePath = settingsBasePath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                            ? settingsBasePath
                            : Request.Scheme + "://" + Request.Host + settingsBasePath;
                        OAuthTokenResponse token = await _canvasApi.RequestToken($"{basePath}/oauth_complete", oAuthId, oAuthKey, code, license.Domain.RemoveHttpProtocolAndTrailingSlash());
                        await _sessionService.UpdateSessionRefreshToken(s, token.access_token, token.refresh_token);

                        return await RedirectToHome(s);
                    }
                    if (provider == LmsProviderNames.Brightspace)
                    {
                        string userId = Request.Query["x_a"];
                        string userKey = Request.Query["x_b"];
                        string token = null;
                        if (!userId.Contains(' ') && !userKey.Contains(' '))
                        {
                            token = userId + " " + userKey;
                        }
                        else
                        {
                            Logger.ErrorFormat("[AuthenticationCallback] UserId:{0}, UserKey:{1}", userId, userKey);
                            this.ViewBag.Error =
                            ViewBag.Error = "Cannot get keys for BrightSpace user";
                            return this.View("~/Views/Lti/LtiError.cshtml");
                        }

                        await _sessionService.UpdateSessionRefreshToken(s, token, token);
                        return await RedirectToHome(s);
                    }
                }
                catch (ApplicationException ex)
                {
                    Logger.ErrorFormat(ex,
                        "[AuthenticationCallback] Application exception. SessionKey:{0}, Message:{1}", session,
                        ex.Message);
                    ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                    ViewBag.Message = ex.Message;
                    return View("~/Views/Lti/LtiError.cshtml");
                }
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", session);
                ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return View("~/Views/Lti/LtiError.cshtml");
            }

        }

        private bool IsValidOAuthOptions(LmsLicenseDto license)
        {
            return license.ZoomUserDto != null;
        }

        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                // TRICK: to save course_id in DB;
                param.CalculateFields();
                // Parse and validate the request
                Request.CheckForRequiredLtiParameters();
                if (!Guid.TryParse(param.oauth_consumer_key, out Guid consumerKey))
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid consumerKey. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException(
                        $"Consumer key is empty or has invalid format.");
                }

                var license = await _licenseService.GetLicense(Guid.Parse(param.oauth_consumer_key));

                if (license != null)
                {
                    if (!IsValidOAuthOptions(license))
                    {
                        Logger.ErrorFormat("Invalid LTI request. Need update license. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                        throw new LtiException($"Please update license.");
                    }

                    //TODO: Add logic to get culture from DB by lmsCompany.LanguageId
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    Logger.ErrorFormat("Zoom integration is not set up. param:{0}.",
                        JsonConvert.SerializeObject(param));
                    throw new LtiException(
                        $"Your Zoom integration is not set up for provided consumer key.");
                }

                if (!(new BltiProviderHelper(Logger)).VerifyBltiRequest(license, Request,
                    () => true)) //todo: remove if not needed
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid signature. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException($"Invalid signature parameter");
                }

                ValidateLtiVersion(param);
                ValidateIntegrationRequiredParameters(license, param);

                LmsUserSession session = await SaveSession(license, param);

                switch ((LMS)license.ProductId)
                {
                    case LMS.Canvas:
                        //NOTE!
                        //Mike wants to use https://canvas.instructure.com/. It is free canvas instance for teacheres.
                        //We cannot get Admin user from this instanse of canvas. Admin user is needed to get OauthId, OauthKey.
                        //So we will use generated token. One techer hasto have only one token. For each techer we have to create a new license.
                        if (license.GetSetting<bool>(LmsLicenseSettingNames.UseGeneratedToken))
                        {
                            break;
                        }

                        var oAuthId = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                        var oAuthKey = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthKey);
                        
                        
                        if (string.IsNullOrWhiteSpace(session?.Token) || await _canvasApi.IsTokenExpired(license.Domain.RemoveHttpProtocolAndTrailingSlash(), session.Token))
                        {
                            if (string.IsNullOrEmpty(session.RefreshToken))
                            {
                                return StartOAuth2Authentication(license, "canvas", session, param);
                            }

                            var refreshParams = new RefreshTokenParamsDto
                            {
                                OAuthId = oAuthId,
                                OAuthKey = oAuthKey,
                                RefreshToken = session.RefreshToken
                            };

                            var accessToken = await _canvasApi.RequestTokenByRefreshToken(refreshParams, license.Domain.RemoveHttpProtocolAndTrailingSlash());
                            if (string.IsNullOrEmpty(accessToken))
                            {
                                return StartOAuth2Authentication(license, "canvas", session, param);
                            }

                            await _sessionService.UpdateSessionAccessToken(session, accessToken);
                        }

                        break;

                    case LMS.AgilixBuzz:
                        break;
                    case LMS.Schoology:
                        break;
                    case LMS.BlackBoard:
                        break;
                    case LMS.Moodle:
                        break;
                    case LMS.Sakai:
                        break;
                    case LMS.Desire2Learn:
                        if (string.IsNullOrEmpty(session.Token))
                        {
                            return StartOAuth2Authentication(license, LmsProviderNames.Brightspace, session, param);
                        }
                        break;
                }

                sw.Stop();
                Logger.InfoFormat($"Metric: LoginWithProvider time: {sw.Elapsed}.\r\n");

                return await RedirectToHome(session);
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
        }

        protected static RestClient CreateRestClient(string api)
        {
            var client = new RestClient(string.Format("{0}{1}", HttpScheme.Https, api));
            return client;
        }

        protected static RestRequest CreateRequest(string api, string resource, Method method, string userToken)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", "Bearer " + userToken);
            return request;
        }


        protected static void Validate(string api, string userToken)
        {
            if (string.IsNullOrWhiteSpace(api))
                throw new ArgumentException("Api can not be empty", nameof(api));

            if (string.IsNullOrWhiteSpace(userToken))
                throw new ArgumentException("UserToken can not be empty", nameof(userToken));
        }

        private static string FixExtraDataIssue(string keyToFix)
        {
            if (keyToFix != null && keyToFix.Contains(","))
            {
                var keys = keyToFix.Split(",".ToCharArray());
                keyToFix = keys.FirstOrDefault() == null ? keyToFix : keys.FirstOrDefault();
            }

            return keyToFix;
        }

        private void ValidateLtiVersion(LtiParamDTO param)
        {
            // in case when client supports v2.0 - just warn, for our AC integration all necessary functionality should be supported
            if (param.lti_version == "")
            {
                Logger.Warn($"[LtiVersion - 2.0] ConsumerKey={param.oauth_consumer_key}");
            }
            //version should match "LTI-1p0" for v1.0, v1.1, v1.2
            else if (param.lti_version != LtiConstants.LtiVersion && param.lti_version != "LTI-1p2"
            ) //bridge uses 1p2, todo: search for correct validation
            {
                Logger.ErrorFormat("Invalid LTI request. Invalid LTI version. oauth_consumer_key:{0}, lti_version:{1}",
                    param.oauth_consumer_key, param.lti_version);
                throw new LtiException("Invalid LTI Version parameter.");
            }
        }

        private void ValidateIntegrationRequiredParameters(LmsLicenseDto license, LtiParamDTO param)
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
            //todo: check if email is obligatory for zoom
            if (string.IsNullOrEmpty(param.lis_person_contact_email_primary))
                missingIntegrationRequiredFields.Add(LtiParameterFriendlyNames.Email);

            if (missingIntegrationRequiredFields.Any())
            {
                throw new LtiException(
                    $"The following parameters are required for Zoom integration: {string.Join(", ", missingIntegrationRequiredFields.ToArray())}");
            }
        }

        private ActionResult StartOAuth2Authentication(LmsLicenseDto lmsLicense, string provider, LmsUserSession session, LtiParamDTO model)
        {
            string schema = Request.Scheme;


            string returnUrl = Url.AbsoluteCallbackAction(schema, new {__provider__ = provider, session = session.Id.ToString() });
            switch (provider)
            {
                case LmsProviderNames.Canvas:
                    var oAuthId = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                    string authUrl = _canvasApi.BuildAuthUrl(returnUrl, model.lms_domain, oAuthId, session.Id.ToString());
                    return Redirect(authUrl);

                case LmsProviderNames.Brightspace:
                    Response.Cookies.Append("providerKey", session.Id.ToString());
                    returnUrl = Url.AbsoluteCallbackAction(schema, new { __provider__ = provider });
                    string redirectUrl = _brightSpaceApi.GetTokenRedirectUrl(new Uri(returnUrl), model.lms_domain, lmsLicense.GetLMSSettings(session)).AbsoluteUri;
                    return Redirect(redirectUrl);
            }

            return null;
        }

        private async Task<LmsUserSession> SaveSession(LmsLicenseDto license, LtiParamDTO param)
        {
            var session = await _sessionService.SaveSession(license.ConsumerKey, param.course_id.ToString(), param,
                          param.lis_person_contact_email_primary, param.lms_user_id);
            return session;
        }

        private async Task<ActionResult> RedirectToHome(LmsUserSession session, StringBuilder trace = null)
        {
            //LtiViewModelDto model = await BuildModelAsync(session, trace);
            return RedirectToAction("Home", "Lti", new
            {
                session = session.Id.ToString()
            });
        }

        private async Task<LtiViewModelDto> BuildModelAsync(LmsUserSession session, StringBuilder trace = null)
        {
            var license = await _licenseService.GetLicense(session.LicenseKey);

            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(session.SessionData);

            //TRICK: we calc shift on serverside
            //acSettings.SetTimezoneShift(null);

            string userFullName = param.lis_person_name_full ??
                                  param.lis_person_name_given + " " + param.lis_person_name_family;

            ZoomUrls.BaseApiUrl = (string) Settings.BaseApiPath.TrimEnd('/');
            ZoomUrls.BaseUrl = (string) Settings.BasePath.TrimEnd('/');

            LmsProvider lmsProvider = LmsProvider.Generate(license.ProductId);

            var model = new LtiViewModelDto
            {

                FullVersion = new Version(Settings.Version),
                // TRICK:
                // BB contains: lis_person_name_full:" Blackboard  Administrator"
                CurrentUserName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline),
                //LicenseSettings = settings,
                IsTeacher = IsTeacher(param),

                CourseMeetingsEnabled =
                    true, //credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,


                LmsProviderName = lmsProvider.LmsProviderName, //todo
                UserGuideLink = $"https://zoomsupport.cosocloud.com/knowledgeBase", //todo
                EnableClassRosterSecurity = license.GetSetting<bool>(LmsLicenseSettingNames.EnableClassRosterSecurity),
                EnableOfficeHours = license.GetSetting<bool>(LmsLicenseSettingNames.EnableOfficeHours),
                EnableOfficeHoursSlots = license.GetSetting<bool>(LmsLicenseSettingNames.EnableOfficeHoursSlots),
                EnableStudyGroups = license.GetSetting<bool>(LmsLicenseSettingNames.EnableStudyGroups),
                EnabledStorageProviders = await GetEnabledStorageProviders(license),
                PrimaryColor = license.GetSetting<string>(LmsLicenseSettingNames.PrimaryColor),
                SupportSectionText = license.GetSetting<string>(LmsLicenseSettingNames.SupportSectionText),
                EnableMeetingSessions = license.GetSetting<bool>(LmsLicenseSettingNames.EnableMeetingSessions)
            };

            return model;
        }

        private async Task<List<int>> GetEnabledStorageProviders(LmsLicenseDto licenseDto)
        {
            var result = new List<int>();

            var enabledKaltura = licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableKaltura);
            if (enabledKaltura)
            {
                result.Add((int)ExternalStorageProvider.Kaltura);
            }

            return result;
        }

        private bool IsTeacher(LtiParamDTO param)
        {
            string teacherRoles = Settings.TeacherRoles;
            return !string.IsNullOrWhiteSpace(teacherRoles) && teacherRoles.Split(',')
                       .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }
}