﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
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
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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

        public LtiController(ILogger logger, ApplicationSettingsProvider settings, ILmsLicenseService licenseService,
            UserSessionService sessionService, IJsonDeserializer jsonDeserializer) : base(logger, settings, sessionService)
        {
            _licenseService = licenseService;
            _jsonDeserializer = jsonDeserializer;
        }

        public async Task<ActionResult> Home(string session)
        {
            try
            {
                //var model = TempData["lti-index-model"] as LtiViewModelDto;

                // TRICK: to change lang inside
                LmsUserSession s = await _sessionService.GetSession(Guid.Parse(session));

                //if (model == null)
                //{
                var    model = await BuildModelAsync(s);
                //}
                return View("Index", model);
            }
            catch (Exception ex) //Core.WarningMessageException
            {
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            
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
                    return View("Error");
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
                        Logger.Error("[AuthenticationCallback] providerKey parameter value is null and there is no cookie with such name");
                        this.ViewBag.Error = "Could not find session information for current user. Please, enable cookies or try to open LTI application in a different browser.";
                        return this.View("Error");
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
                        var license = _licenseService.GetLicense(param.oauth_consumer_key);
                        var oAuthId = license.GetSetting<string>(LmsLicenseSettingNames.OAuthAppId);
                        var oAuthKey = license.GetSetting<string>(LmsLicenseSettingNames.OAuthAppKey);

                        IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                        pairs.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
                        pairs.Add(new KeyValuePair<string, string>("client_id", oAuthId));
                        pairs.Add(new KeyValuePair<string, string>("redirect_uri", $"{Settings.BasePath}/oauth_complete"));
                        pairs.Add(new KeyValuePair<string, string>("client_secret", oAuthKey));
                        pairs.Add(new KeyValuePair<string, string>("code", code));


                        HttpClient httpClient = new HttpClient();
                        var httpResponseMessage = await httpClient.PostAsync($"https://{license.Domain}/login/oauth2/token", new FormUrlEncodedContent(pairs));
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            if (provider.ToLower() == LmsProviderNames.Canvas)
                            {
                                if (param.lms_user_login == "$Canvas.user.loginId")
                                    throw new InvalidOperationException("[Canvas Authentication Error]. Please login to Canvas.");
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
                        ResponseTocken responseTocken = _jsonDeserializer.JsonDeserialize<ResponseTocken>(resp);
                        string tocken = responseTocken.access_token;
                        await _sessionService.UpdateSessionToken(s, tocken);

                        return await RedirectToHome(s);

                    }

                }
                catch (ApplicationException ex)
                {
                    Logger.ErrorFormat(ex, "[AuthenticationCallback] Application exception. SessionKey:{0}, Message:{1}", session, ex.Message);
                    ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                    ViewBag.Message = ex.Message;
                    return View("~/Views/Lti/LtiError.cshtml");
                }
                return this.View("Error");

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

                var license = _licenseService.GetLicense(param.oauth_consumer_key);

                if (license != null)
                {
                    //TODO: Add logic to get culture from DB by lmsCompany.LanguageId
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    Logger.ErrorFormat("Adobe Connect integration is not set up. param:{0}.",
                        JsonConvert.SerializeObject(param));
                    throw new LtiException(
                        $"Invalid LTI request. Your Adobe Connect integration is not set up for provided consumer key.");
                }

                string validationError = ValidateLmsLicense(license, param);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    this.ViewBag.Error = validationError;
                    return this.View("Error");
                }

                LmsProvider providerInstance = LmsProvider.Generate();

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

                switch ((LmsProviderEnum)license.LmsProviderId)
                {
                    case LmsProviderEnum.Canvas:

                        sw = Stopwatch.StartNew();

                        if (string.IsNullOrWhiteSpace(session?.Token) || await IsTokenExpired(license.Domain, session.Token))
                        {
                            return this.StartOAuth2Authentication(license, "canvas", sessionKey, param);
                        }

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
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.",
                    param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
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
                    $"The following parameters are required for AC integration: {string.Join(", ", missingIntegrationRequiredFields.ToArray())}");
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

                    var oAuthId = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.OAuthAppId);
                    var oAuthKey = lmsLicense.GetSetting<string>(LmsLicenseSettingNames.OAuthAppKey);
                    returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, session);
                    var oAuthSettings = OAuthWebSecurityWrapper.GetOAuthSettings(lmsLicense, (string)Settings.CanvasClientId, (string)Settings.CanvasClientSecret);
                    if (string.IsNullOrEmpty(oAuthSettings.Key) || string.IsNullOrEmpty(oAuthSettings.Value))
                    {
                        var message = "Invalid OAuth parameters. Application Id and Application Key cannot be empty.";
                        throw new LtiException(message);
                    }
                    ////////
                    Uri uri = new Uri(returnUrl);
                    var baseUri = uri.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
                    var query = QueryHelpers.ParseQuery(uri.Query);
                    var items = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                    string parameterValue = Guid.NewGuid().ToString("N");
                    var qb = new QueryBuilder(items);
                    qb.Add("__sid__", parameterValue);
                    qb.Add("__provider__", "cancas");
                    var fullUri = baseUri + qb.ToQueryString();


                    var builder = new UriBuilder("https://esynctraining.instructure.com/login/oauth2/auth");

                    var parameters = new Dictionary<string, string>
                    {
                        { "client_id", oAuthId },
                        { "redirect_uri", fullUri },
                        { "response_type", "code" },
                        { "state", Convert.ToBase64String(Encoding.ASCII.GetBytes("esynctraining.instructure.com" + "&&&ru="  + fullUri)) }
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
            var session = await _sessionService.GetSession(license.Id, param.course_id.ToString(), param.lms_user_id);
            session = session ?? await _sessionService.SaveSession(license.Id, param.course_id.ToString(), param,
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
                //tab = tab,
                //meetingId = ltiId
            });
        }

        private async Task<LtiViewModelDto> BuildModelAsync(LmsUserSession session, StringBuilder trace = null)
        {
            var sw = Stopwatch.StartNew();

            //var credentials = session.LmsCompany;
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(session.SessionData);
            
            //TRICK: we calc shift on serverside
            //acSettings.SetTimezoneShift(null);

            string userFullName = param.lis_person_name_full ?? param.lis_person_name_given + " " + param.lis_person_name_family;
            //var settings = LicenseSettingsDto.Build(credentials, LanguageModel.GetById(credentials.LanguageId), Cache);

            //var filePattern = (string)Settings.JsBuildSelector;
            //var versionFileJs = CacheUtility.GetCachedItem<Version>(PersistantCache, CachePolicies.Keys.VersionFileName(filePattern), () =>
            //{
            //    return VersionProcessor.ProcessVersion(Server.MapPath("~/extjs/"), filePattern);
            //});

            ZoomUrls.BaseUrl = (string)Settings.BaseApiPath.TrimEnd('/');

            //var lmsProvider = LmsProviderModel.GetById(credentials.LmsProviderId);
            var model = new LtiViewModelDto
            {
                FullVersion = new Version(0, 6, 0, 0),//versionFileJs,
                //                LtiVersion = version,

                // TRICK:
                // BB contains: lis_person_name_full:" Blackboard  Administrator"
                CurrentUserName = Regex.Replace(userFullName.Trim(), @"\s+", " ", RegexOptions.Singleline),
                //LicenseSettings = settings,
                IsTeacher = IsTeacher(param),

                CourseMeetingsEnabled = true,//credentials.EnableCourseMeetings.GetValueOrDefault() || param.is_course_meeting_enabled,

                LmsProviderName = "Canvas",//lmsProvider.LmsProviderName,
                UserGuideLink = "https://stage.edugamecloud.com/content/lti-instructions/ZoomIntegration.pdf"
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
    }
}