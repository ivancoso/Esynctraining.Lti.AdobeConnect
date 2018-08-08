using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Api;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Constants;
using Esynctraining.Lti.Zoom.Core.Extensions;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Lti.Zoom.Domain.Entities;
using Esynctraining.Lti.Zoom.DTO;
using Esynctraining.Lti.Zoom.Extensions;
using Esynctraining.Lti.Zoom.OAuth;
using Esynctraining.Lti.Zoom.OAuth.Canvas;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RestSharp;
using HttpScheme = Esynctraining.Lti.Zoom.Constants.HttpScheme;
using ILogger = Esynctraining.Core.Logging.ILogger;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public class LtiController : BaseController
    {
        private const string ProviderKeyCookieName = "providerKey";

        private readonly ILmsLicenseService _licenseService;
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly ZoomMeetingService _meetingService;
        private readonly ZoomUserService _userService;
        private readonly LmsUserServiceFactory _lmsUserServiceFactory;

        public LtiController(ILogger logger, ApplicationSettingsProvider settings, ILmsLicenseService licenseService,
            UserSessionService sessionService, IJsonDeserializer jsonDeserializer, ZoomMeetingService meetingService,
            ZoomUserService userService, LmsUserServiceFactory lmsUserServiceFactory) : base(logger, settings, sessionService)
        {
            _licenseService = licenseService;
            _jsonDeserializer = jsonDeserializer;
            _meetingService = meetingService;
            _userService = userService;
            _lmsUserServiceFactory = lmsUserServiceFactory;
        }

        [HttpGet]
        public virtual async Task<ActionResult> JoinMeeting(int meetingId, string session)
        {
            var s = await GetSession(session);
            var license = await _licenseService.GetLicense(s.LicenseKey);
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);
            var dbMeeting = await _meetingService.GetMeeting(meetingId, param.course_id.ToString());

            if (dbMeeting == null)
                return NotFound(meetingId);
            UserInfoDto zoomUser = null;
            string userId;
            try
            {
                /*
                 {
"code": 1010,
"message": "User not belong to this account"
}*/
                zoomUser = _userService.GetUser(param.lis_person_contact_email_primary);
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
                /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                var userInfo = _userService.CreateUser(new CreateUserDto
                {
                    Email = param.lis_person_contact_email_primary,
                    FirstName = param.PersonNameGiven,
                    LastName = param.PersonNameFamily
                });

                return Content(
                    "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
            }
            if (zoomUser != null)
            {
                var url = await _meetingService.GetMeetingUrl(zoomUser.Id, dbMeeting.ProviderMeetingId,
                    param.lis_person_contact_email_primary,
                    async () =>
                    {
                        var settings = GetSettings(s, license);
                        var lmsService = _lmsUserServiceFactory.GetUserService(license.ProductId);
                        var lmsUserExistsInCourse = await lmsService.GetUser(settings, s.LmsUserId, s.CourseId);
                        if (!lmsUserExistsInCourse.IsSuccess)
                        {
                            Logger.Warn(
                                $"[JoinMeeting:{meetingId}] LmsUserId:{s.LmsUserId}, Message:{lmsUserExistsInCourse.Message}");
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

            return Content("Error when joining.");
        }

        public async Task<ActionResult> Home(string session)
        {
            LmsUserSession s = await _sessionService.GetSession(Guid.Parse(session));
            var license = await _licenseService.GetLicense(s.LicenseKey);
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);
            var activeUsers = _userService.GetUsers(UserStatuses.Active);
            if (!activeUsers.Users.Any(u =>
                u.Email.Equals(param.lis_person_contact_email_primary, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    var userInfo = _userService.CreateUser(new CreateUserDto
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.PersonNameGiven,
                        LastName = param.PersonNameFamily
                    });
                }

                catch (ZoomApiException ex)
                {
                    /*{
    "code": 1005,
    "message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
    }*/
                    Logger.Error($"[ZoomApiException] Status:{ex.StatusDescription}, Content:{ex.Content}, ErrorMessage: {ex.ErrorMessage}", ex);
                }

                ViewBag.Message = "Your account is either in 'pending' or 'inactive' status. Check email for registration link or contact your Zoom account manager.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            var model = await BuildModelAsync(s);
            return View("Index", model);
        }

        public virtual async Task<ActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            return this.View("About");
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification =
            "Reviewed. Suppression is OK here.")]
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
                    //if (provider == LmsProviderNames.Canvas)
                    {
                        var license = await _licenseService.GetLicense(Guid.Parse(param.oauth_consumer_key));
                        var oAuthId = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                        var oAuthKey = license.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthKey);

                        IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                        pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                        pairs.Add(new KeyValuePair<string, string>("client_id", oAuthId));
                        pairs.Add(new KeyValuePair<string, string>("redirect_uri",
                            $"{Settings.BasePath}/oauth_complete"));
                        pairs.Add(new KeyValuePair<string, string>("client_secret", oAuthKey));
                        pairs.Add(new KeyValuePair<string, string>("code", code));


                        System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
                        var httpResponseMessage = await httpClient.PostAsync(
                            $"https://{license.Domain}/login/oauth2/token", new FormUrlEncodedContent(pairs));
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            if (provider.ToLower() == LmsProviderNames.Canvas)
                            {
                                if (param.lms_user_login == "$Canvas.user.loginId")
                                    throw new InvalidOperationException(
                                        "[Canvas Authentication Error]. Please login to Canvas.");
                            }
                        }
                        else
                        {
                            var sid = Request.Query["__sid__"].ToString();
                            var cookie = Request.Cookies[sid];

                            this.ViewBag.Error = string.Format(
                                "Generic OAuth fail: code:[{0}] provider:[{1}] sid:[{2}] cookie:[{3}]",
                                Request.Query["code"],
                                Request.Query["__provider__"],
                                sid);
                        }


                        var resp = await httpResponseMessage.Content.ReadAsStringAsync();
                        ResponseToken responseToken = _jsonDeserializer.JsonDeserialize<ResponseToken>(resp);
                        string tocken = responseToken.access_token;
                        await _sessionService.UpdateSessionToken(s, tocken);

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
            //catch (Core.WarningMessageException ex)
            //{
            //    Logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", session);
            //    ViewBag.Message = ex.Message;
            //    return View("~/Views/Lti/LtiError.cshtml");
            //}
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[AuthenticationCallback] exception. SessionKey:{0}.", session);
                ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return View("~/Views/Lti/LtiError.cshtml");
            }

            ViewData["Message"] = __provider__;
            return this.View("About");
        }

        public virtual async Task<ActionResult> LoginWithProvider(string provider, LtiParamDTO param)
        {
            var methodTime = Stopwatch.StartNew();
            var trace = new StringBuilder();

            try
            {
                // TRICK: to save course_id in DB;
                param.CalculateFields();
                // Parse and validate the request
                Request.CheckForRequiredLtiParameters();
                var sw = Stopwatch.StartNew();

                var license = await _licenseService.GetLicense(Guid.Parse(param.oauth_consumer_key));

                if (license != null)
                {
                    //TODO: Add logic to get culture from DB by lmsCompany.LanguageId
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    Logger.ErrorFormat("Zoom integration is not set up. param:{0}.",
                        JsonConvert.SerializeObject(param));
                    throw new LtiException(
                        $"Invalid LTI request. Your Zoom integration is not set up for provided consumer key.");
                }

                string validationError = ValidateLmsLicense(license, param);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    this.ViewBag.Error = validationError;
                    return this.View("~/Views/Lti/LtiError.cshtml");
                }

                //LmsProvider providerInstance = LmsProvider.Generate();

                sw = Stopwatch.StartNew();

                if (!(new BltiProviderHelper(Logger)).VerifyBltiRequest(license, Request,
                    () => true)) //todo: remove if not needed
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid signature. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException($"Invalid LTI request. Invalid signature parameter");
                }

                sw.Stop();
                trace.AppendFormat("VerifyBltiRequest: time: {0}.\r\n", sw.Elapsed.ToString());

                ValidateLtiVersion(param);
                ValidateIntegrationRequiredParameters(license, param);

                LmsUserSession session = await SaveSession(license, param);
                var sessionKey = session.Id.ToString();

                switch ( license.ProductId)
                {
                    case 1010:

                        sw = Stopwatch.StartNew();

                        if (string.IsNullOrWhiteSpace(session?.Token) ||
                            await IsTokenExpired(license.Domain, session.Token))
                        {
                            return this.StartOAuth2Authentication(license, "canvas", sessionKey, param);
                        }

                        break;

                    case 1020:
                        break;
                }

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
            //catch (Core.WarningMessageException ex)
            //{
            //    _logger.WarnFormat("[WarningMessageException] param:{0}.",
            //        JsonSerializer.JsonSerialize(param));
            //    this.ViewBag.Message = ex.Message;
            //    return this.View("~/Views/Lti/LtiError.cshtml");
            //}
            //catch (Exception ex)
            //{
            //    Logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.",
            //        param.oauth_consumer_key);
            //    this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
            //    return this.View("~/Views/Lti/LtiError.cshtml");
            //}
            {
                methodTime.Stop();
                var time = methodTime.Elapsed;
                if (time > TimeSpan.FromSeconds(int.Parse((string) Settings.Monitoring_MaxLoginTime)))
                {
                    var monitoringLog = IoC.Resolve<ILogger>("Monitoring");

                    monitoringLog.ErrorFormat("LoginWithProvider takes more than {0} seconds. Time: {1}. Details: {2}.",
                        Settings.Monitoring_MaxLoginTime.ToString(),
                        time.ToString(), trace.ToString());
                }
            }

            ViewData["Message"] = provider;
            return this.View("~/Views/Lti/About.cshtml");
        }

        private void ValidateLoggedUser(string paramLisPersonContactEmailPrimary)
        {
            var activeUsers = _userService.GetUsers(UserStatuses.Active);
            if (activeUsers.Users.Any(u => u.Email.Equals(paramLisPersonContactEmailPrimary, StringComparison.CurrentCultureIgnoreCase)))
                return;

            var inactiveUsers = _userService.GetUsers(UserStatuses.Inactive);
            if (inactiveUsers.Users.Any(u => u.Email.Equals(paramLisPersonContactEmailPrimary, StringComparison.CurrentCultureIgnoreCase)))
                throw new LtiException("User has an account, but it is inactive.");

            var pendingUsers = _userService.GetUsers(UserStatuses.Pending);
            if (pendingUsers.Users.Any(u => u.Email.Equals(paramLisPersonContactEmailPrimary, StringComparison.CurrentCultureIgnoreCase)))
                throw new LtiException("User has an account, but it is not activated yet.");


            throw new LtiException("User doesn't have zoom account.");
        }

        public async Task<bool> IsTokenExpired(string api, string userToken)
        {
            try
            {
                Validate(api, userToken);

                var client = CreateRestClient(api);

                RestRequest request = CreateRequest(
                    api,
                    "/api/v1/users/self/profile",
                    Method.GET,
                    userToken);

                IRestResponse<LmsUserDTO> response = client.Execute<LmsUserDTO>(request);

                return response.StatusCode == HttpStatusCode.Unauthorized;

                //-------------------------
                //var builder = new UriBuilder("https://esynctraining.instructure.com" + "/api/v1/users/self/profile");

                //HttpClient httpClient = new HttpClient();
                //var requestMessage = new HttpRequestMessage(HttpMethod.Get, builder.Uri.AbsoluteUri);
                //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer " + userToken);
                //HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                //return response.StatusCode == HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "[CanvasAPI.IsTokenExpired] API:{0}. UserToken:{1}.", api, userToken);
                throw;
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

        private string ValidateLmsLicense(LmsLicenseDto lmsLicense, LtiParamDTO param)
        {
            if (!true)
                //TODO update
                //if (!lmsLicense.HasLmsDomain(param.lms_domain))
            {
                Logger.ErrorFormat(
                    "LTI integration is already set for different domain. Request's lms_domain:{0}. oauth_consumer_key:{1}.",
                    param.lms_domain, param.oauth_consumer_key);
                return "This LTI integration is already set for different domain.";
            }

            //if (!lmsLicense.IsActive)
            //{
            //    _logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
            //    return "LMS License is not active. Please contact administrator.";
            //}


            if (!true)
                //TODO update
                //if (!CompanyModel.IsActive(lmsLicense.CompanyId))
            {
                Logger.ErrorFormat("Company doesn't have any active license. oauth_consumer_key:{0}.",
                    param.oauth_consumer_key);
                return "Sorry, your company doesn't have any active license. Please contact administrator.";
            }

            return null;
        }

        private ActionResult StartOAuth2Authentication(LmsLicenseDto lmsLicense, string provider, string session,
            LtiParamDTO model)
        {
            string schema = Request.Scheme;


            string returnUrl = Url.AbsoluteCallbackAction(schema, new {__provider__ = provider, session});
            switch (provider)
            {
                case LmsProviderNames.Canvas:
                    returnUrl = UriBuilderExtensions.AddQueryStringParameter(
                        returnUrl, Core.Utils.Constants.ReturnUriExtensionQueryParameterName,
                        HttpScheme.Https + model.lms_domain);

                    var oAuthId = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthId);
                    var oAuthKey = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.CanvasOAuthKey);
                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, session);
                   
                    if (string.IsNullOrEmpty(oAuthId) || string.IsNullOrEmpty(oAuthKey))
                    {
                        var message = "Invalid OAuth parameters. Application Id and Application Key cannot be empty.";
                        throw new LtiException(message);
                    }
                    
                    Uri uri = new Uri(returnUrl);
                    var baseUri =
                        uri.GetComponents(
                            UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path,
                            UriFormat.UriEscaped);
                    var query = QueryHelpers.ParseQuery(uri.Query);
                    var items = query.SelectMany(x => x.Value,
                        (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                    string parameterValue = Guid.NewGuid().ToString("N");
                    var qb = new QueryBuilder(items);
                    qb.Add("__sid__", parameterValue);
                    qb.Add("__provider__", "canvas");
                    var fullUri = baseUri + qb.ToQueryString();


                    var builder = new UriBuilder($"https://{lmsLicense.Domain}/login/oauth2/auth");

                    var parameters = new Dictionary<string, string>
                    {
                        {"client_id", oAuthId},
                        {"redirect_uri", fullUri},
                        {"response_type", "code"},
                        {
                            "state",
                            Convert.ToBase64String(
                                Encoding.ASCII.GetBytes($"{lmsLicense.Domain}&&&ru={fullUri}"))
                        }
                    };

                    foreach (var key in parameters.Keys)
                    {
                        builder.AppendQueryArgument(key, parameters[key]);
                    }

                    return Redirect(builder.Uri.AbsoluteUri);
                    break;
            }

            return null;
        }

        private async Task<LmsUserSession> SaveSession(LmsLicenseDto license, LtiParamDTO param)
        {
            //var session = await _sessionService.GetSession(license.ConsumerKey, param.course_id.ToString(), param.lms_user_id);
            var session = await _sessionService.SaveSession(license.ConsumerKey, param.course_id.ToString(), param,
                          param.lis_person_contact_email_primary, param.lms_user_id);
            return session;
        }

        private async Task<ActionResult> RedirectToHome(LmsUserSession session, StringBuilder trace = null)
        {
            LtiViewModelDto model = await BuildModelAsync(session, trace);
            //TempData["lti-index-model"] = model;

            //var primaryColor = session.LmsUser.PrimaryColor;
            //primaryColor = !string.IsNullOrWhiteSpace(primaryColor) ? primaryColor : (session.LmsCompany.PrimaryColor ?? string.Empty);

            return RedirectToAction("Home", "Lti", new
            {
                //primaryColor = primaryColor,
                session = session.Id.ToString(),
                disableCacheBuster = true,
                email = session.Email,
                //tab = tab,
                //meetingId = ltiId
            });
        }

        private async Task<LtiViewModelDto> BuildModelAsync(LmsUserSession session, StringBuilder trace = null)
        {
            var license = await _licenseService.GetLicense(session.LicenseKey);

            //var credentials = session.LmsCompany;
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(session.SessionData);

            //TRICK: we calc shift on serverside
            //acSettings.SetTimezoneShift(null);

            string userFullName = param.lis_person_name_full ??
                                  param.lis_person_name_given + " " + param.lis_person_name_family;
            //var settings = LicenseSettingsDto.Build(credentials, LanguageModel.GetById(credentials.LanguageId), Cache);

            //var filePattern = (string)Settings.JsBuildSelector;
            //var versionFileJs = CacheUtility.GetCachedItem<Version>(PersistantCache, CachePolicies.Keys.VersionFileName(filePattern), () =>
            //{
            //    return VersionProcessor.ProcessVersion(Server.MapPath("~/extjs/"), filePattern);
            //});

            ZoomUrls.BaseApiUrl = (string) Settings.BaseApiPath.TrimEnd('/');
            ZoomUrls.BaseUrl = (string) Settings.BasePath.TrimEnd('/');

            //var lmsProvider = LmsProviderModel.GetById(credentials.LmsProviderId);
            var model = new LtiViewModelDto
            {
                FullVersion = new Version(0, 6, 0, 0), //versionFileJs,
                //                LtiVersion = version,

                // TRICK:
                // BB contains: lis_person_name_full:" Blackboard  Administrator"
                CurrentUserName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline),
                //LicenseSettings = settings,
                IsTeacher = IsTeacher(param),

                CourseMeetingsEnabled =
                    true, //credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,

                LmsProviderName = "Canvas", //lmsProvider.LmsProviderName,
                UserGuideLink = $"{ZoomUrls.BaseUrl}/ZoomIntegration.pdf",
                EnableClassRosterSecurity = license.GetSetting<bool>(LmsLicenseSettingNames.EnableClassRosterSecurity),
                EnableOfficeHours = license.GetSetting<bool>(LmsLicenseSettingNames.EnableOfficeHours),
                PrimaryColor = license.GetSetting<string>(LmsLicenseSettingNames.PrimaryColor),
                SupportSectionText = license.GetSetting<string>(LmsLicenseSettingNames.SupportSectionText)
                /*!string.IsNullOrEmpty(lmsProvider.UserGuideFileUrl)
                    ? lmsProvider.UserGuideFileUrl
                    : new Uri(new Uri((string)Settings.BasePath, UriKind.Absolute), $"content/lti-instructions/{lmsProvider.LmsProviderName}.pdf").ToString(),*/
            };

            return model;
        }

        private bool IsTeacher(LtiParamDTO param)
        {
            string teacherRoles = Settings.TeacherRoles;
            return !string.IsNullOrWhiteSpace(teacherRoles) && teacherRoles.Split(',')
                       .Any(x => param.roles.IndexOf(x.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        private bool IsAdminRole(LtiParamDTO param)
        {
            if (param == null)
            {
                return this.IsDebug;
            }

            return param.roles.Contains("Administrator");
        }

        private Dictionary<string, object> GetSettings(LmsUserSession session, LmsLicenseDto license)
        {
            Dictionary<string, object> result = null;
            List<string> optionNamesForCanvas;
            if (license.ProductId == 1010)
            {
                optionNamesForCanvas = new List<string> { LmsLicenseSettingNames.CanvasOAuthId, LmsLicenseSettingNames.CanvasOAuthKey };
                result = license.Settings.Where(x => optionNamesForCanvas.Any(o => o == x.Key)).ToDictionary(k => k.Key, v => (object)v.Value);
                result.Add(LmsLicenseSettingNames.LicenseKey, license.ConsumerKey);
                result.Add(LmsLicenseSettingNames.LmsDomain, license.Domain);
                result.Add(LmsUserSettingNames.Token, session.Token);
            }
            if (license.ProductId == 1020)
            {
                optionNamesForCanvas = new List<string> { LmsLicenseSettingNames.BuzzAdminUsername, LmsLicenseSettingNames.BuzzAdminPassword };
                result = license.Settings.Where(x => optionNamesForCanvas.Any(o => o == x.Key)).ToDictionary(k => k.Key, v => (object)v.Value);
                result.Add(LmsLicenseSettingNames.LicenseKey, license.ConsumerKey);
                result.Add(LmsLicenseSettingNames.LmsDomain, license.Domain);
            }

            return result;
        }
    }
}