using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;
using HttpScheme = Esynctraining.Lti.Zoom.Constants.HttpScheme;
using ILogger = Esynctraining.Core.Logging.ILogger;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public class LtiController : BaseController
    {
        private readonly ILmsLicenseService _licenseService;
        private readonly UserSessionService _sessionService;
        private readonly IJsonDeserializer _jsonDeserializer;

        public LtiController(ILogger logger, ApplicationSettingsProvider settings, ILmsLicenseService licenseService,
            UserSessionService sessionService, IJsonDeserializer jsonDeserializer) : base(logger, settings)
        {
            _licenseService = licenseService;
            _sessionService = sessionService;
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

                        //sw = Stopwatch.StartNew();

                        //        if (string.IsNullOrWhiteSpace(lmsUser?.Token) ||
                        //            CanvasApi.IsTokenExpired(lmsCompany.LmsDomain, lmsUser.Token))
                        //        {
                        this.StartOAuth2Authentication(license, "Canvas", "1251215", param);
                        return null;
                        //        }

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

        private void StartOAuth2Authentication(LmsLicenseDto lmsLicense, string provider, string session,
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

                    var oAuthId = lmsLicense.GetSetting<string>(LmsCompanySettingNames.OAuthAppId);
                    var oAuthKey = lmsLicense.GetSetting<string>(LmsCompanySettingNames.OAuthAppKey);
                    //returnUrl = CanvasClient.AddProviderKeyToReturnUrl(returnUrl, session);
                    //var oAuthSettings = OAuthWebSecurityWrapper.GetOAuthSettings(lmsCompany, (string)_settings.CanvasClientId, (string)_settings.CanvasClientSecret);
                    //if (string.IsNullOrEmpty(oAuthSettings.Key) || string.IsNullOrEmpty(oAuthSettings.Value))
                    //{
                    //    var message = "Invalid OAuth parameters. Application Id and Application Key cannot be empty.";
                    //    throw new LtiException(message);
                    //}
                    //OAuthWebSecurityWrapper.RequestAuthentication(HttpContext, oAuthSettings, returnUrl);
                    break;
            }
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