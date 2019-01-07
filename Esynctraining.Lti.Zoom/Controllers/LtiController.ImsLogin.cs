using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.Dto.Outcomes;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Lti.Zoom.Extensions;
using Esynctraining.Lti.Zoom.OAuth;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public partial class LtiController
    {
        //[ActionName("ims")]
        //[OutputCache(VaryByParam = "*", NoStore = true, Duration = 0,Location = System.Web.UI.OutputCacheLocation.None)]
        //[ValidateInput(false)]
        public virtual async Task<ActionResult> ImsLogin(LtiParamDTO param)
        {
            try
            {
                Request.CheckForRequiredLtiParameters();
                LmsLicenseDto license;
                if (!Guid.TryParse(param.oauth_consumer_key, out Guid consumerKey))
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid consumerKey. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException(
                        $"Consumer key is empty or has invalid format.");
                }

                license = await _licenseService.GetLicense(Guid.Parse(param.oauth_consumer_key));

                if (license != null)
                {
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

//                if (!lmsCompany.IsActive)
//                {
//                    Logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.",
//                        param.lms_domain, param.oauth_consumer_key);
//                    throw new LtiException(Resources.Messages.LtiValidationInactiveLmsLicense);
//                }

                ValidateLtiVersion(param);

                if (!(new BltiProviderHelper(Logger)).VerifyBltiRequest(license, Request, () => true))
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid signature. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException($"Invalid signature.");
                }


                ValidateIntegrationRequiredParameters(license, param);

                var contextRoles = GetRoles(param.roles);
                if (!contextRoles.Any())
                    throw new LtiException("There is no LIS context role provided in request.");

                LmsUserSession session = await SaveSession(license, param);

                await CreateZoomAccount(param, license);

                var sessionKey = session.Id.ToString();
                if (!string.IsNullOrWhiteSpace(param.lis_outcome_service_url) &&
                    !string.IsNullOrWhiteSpace(param.lis_result_sourcedid))
                {
                    return View("~/Views/Lti/Outcomes.cshtml", new OutcomeModel {Session = sessionKey});
                }
                //this.meetingSetup.SetupFolders(lmsCompany, adobeConnectProvider);

//                Principal acPrincipal = null;
//
//                acPrincipal = acUserService.GetOrCreatePrincipal(
//                    adobeConnectProvider,
//                    param.lms_user_login,
//                    param.lis_person_contact_email_primary,
//                    param.PersonNameGiven,
//                    param.PersonNameFamily,
//                    lmsCompany);
//                if (lmsUser == null)
//                {
//                    lmsUser = new LmsUser
//                    {
//                        LmsCompany = lmsCompany,
//                        UserId = param.lms_user_id,
//                        Username = param.GetUserNameOrEmail(),
//                        PrincipalId = acPrincipal?.PrincipalId,
//                    };
//                    this.lmsUserModel.RegisterSave(lmsUser);
//
//                    // TRICK: save lmsUser to session
//                    SaveSessionUser(session, lmsUser);
//                }
//
//                if (acPrincipal != null && !acPrincipal.PrincipalId.Equals(lmsUser.PrincipalId))
//                {
//                    lmsUser.PrincipalId = acPrincipal.PrincipalId;
//                    this.lmsUserModel.RegisterSave(lmsUser);
//                }
//
//                if (acPrincipal == null)
//                {
//                    Logger.ErrorFormat(
//                        "[LoginWithProvider] Unable to create AC account. LmsCompany ID: {0}. LmsUserID: {1}. lms_user_login: {2}.",
//                        lmsCompany.Id, lmsUser.Id, param.lms_user_login);
//                    throw new Core.WarningMessageException(Resources.Messages.LtiNoAcAccount);
//                }

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
//            catch (Core.WarningMessageException ex)
//            {
//                Logger.WarnFormat("[WarningMessageException] param:{0}.",
//                    JsonSerializer.JsonSerialize(param));
//                this.ViewBag.Message = ex.Message;
//                return this.View("~/Views/Lti/LtiError.cshtml");
//            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.",
                    param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        //Outcomes: copy-pasted with adding eSync-lti-zoom specifics, needed for certification only
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Outcomes(string session)
        {
            if (string.IsNullOrEmpty(session))
            {
                this.ViewBag.Message = "Required parameters were not provided. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            var model = new OutcomeModel
            {
                Session = session
            };
            return View("~/Views/Lti/Outcomes.cshtml", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> Outcomes(OutcomeModel model, string submit)
        {
            if (string.IsNullOrEmpty(model.Session))
            {
                this.ViewBag.Message = "Session parameter is required. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            var userSession = await GetSession(model.Session);
            var lmsLicense = await _licenseService.GetLicense(userSession.LicenseKey);
            var param = _jsonDeserializer.JsonDeserialize<LtiParamDTO>(userSession.SessionData);

            var outcomeUrl = param.lis_outcome_service_url;
            var outcomeSourceId = param.lis_result_sourcedid;
            if (string.IsNullOrWhiteSpace(outcomeUrl) || string.IsNullOrWhiteSpace(outcomeSourceId))
            {
                this.ViewBag.Message =
                    "Outcomes service is unavailable for current session. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                switch (submit)
                {
                    case "Send Grade":
                        var sendResponse = await Outcomes1Client.ReplaceResultAsync(httpClient, outcomeUrl,
                            lmsLicense.ConsumerKey.ToString(), lmsLicense.SharedSecret.ToString(),
                            outcomeSourceId, model.Score);
                        if (sendResponse.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.Message = "Grade sent";
                        }
                        else
                        {
                            ViewBag.Message = $"Invalid request";
                        }
                        break;
                    case "Read Grade":
                        var readResponse = await Outcomes1Client.ReadResultAsync(httpClient, outcomeUrl,
                            lmsLicense.ConsumerKey.ToString(), lmsLicense.SharedSecret.ToString(),
                            outcomeSourceId);
                        if (readResponse.StatusCode == HttpStatusCode.OK)
                        {
                            model.Score = readResponse.Response.Score;
                            ViewBag.Message = "Grade read";
                        }
                        else
                        {
                            ViewBag.Message = "No grade";
                        }
                        break;
                    case "Delete Grade":
                        var deleteResponse = await Outcomes1Client.DeleteResultAsync(httpClient, outcomeUrl,
                            lmsLicense.ConsumerKey.ToString(), lmsLicense.SharedSecret.ToString(),
                            outcomeSourceId);
                        if (deleteResponse.StatusCode == HttpStatusCode.OK)
                        {
                            model.Score = null;
                            ViewBag.Message = "Grade deleted";
                        }
                        else
                        {
                            ViewBag.Message = "Invalid request";
                        }
                        break;
                }
            }

            return View("~/Views/Lti/Outcomes.cshtml", model);
        }

        private IList<Enum> GetRoles(string rolesParam)
        {
            var roles = new List<Enum>();
            if (string.IsNullOrWhiteSpace(rolesParam))
            {
                return roles;
            }

            foreach (var roleValue in rolesParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(roleValue, true, out ContextRole role))
                {
                    roles.Add(role);
                }
                else
                {
                    if (LtiConstants.RoleUrns.ContainsKey(roleValue))
                    {
                        roles.Add(LtiConstants.RoleUrns[roleValue]);
                    }
                }
            }

            return roles;
        }

        private async Task CreateZoomAccount(LtiParamDTO param, LmsLicenseDto license)
        {
            var zoomApi = new ZoomApiWrapper(new ZoomApiOptions
            {
                ZoomApiKey = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
                ZoomApiSecret = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret)
            });

            var activeUserEmails = (GetUsersFromApi(UserStatus.Active, zoomApi)).Where(x => !string.IsNullOrEmpty(x.Email)).Select(x => x.Email);

            if (!activeUserEmails.Any(x =>
                x.Equals(param.lis_person_contact_email_primary, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    var userInfo =
                        zoomApi.CreateUser(new CreateUser
                        {
                            Email = param.lis_person_contact_email_primary,
                            FirstName = param.PersonNameGiven,
                            LastName = param.PersonNameFamily,
                            Type = IsTeacher(param) ? UserTypes.Pro : UserTypes.Basic,
                        }, "custCreate");
                    await _cacheUpdater.UpdateUsers(license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey), zoomApi);
                }

                catch (ZoomApiException ex)
                {
                    Logger.Error(
                        $"[ZoomApiException] Status:{ex.StatusDescription}, Content:{ex.Content}, ErrorMessage: {ex.ErrorMessage}",
                        ex);
                }
            }
        }

        public List<User> GetUsersFromApi(UserStatus status, ZoomApiWrapper apiWrapper)
        {
            var users = new List<User>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var page = apiWrapper.GetUsers(status, pageSize: pageSize, pageNumber: pageNumber);
                users.AddRange(page.Users);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);

            return users;
        }
    }
}
