using System;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.Core.OAuth;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using EdugameCloud.Lti.Models;
using Esynctraining.AC.Provider.Entities;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Outcomes;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiController
    {
        [ActionName("ims")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual ActionResult ImsLogin(LtiParamDTO param)
        {
            try
            {
                Request.CheckForRequiredLtiParameters();

                LmsCompany lmsCompany =
                    lmsCompanyModel.GetOneByConsumerKey(param.oauth_consumer_key)
                        .Value;

                if (lmsCompany != null)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo(LanguageModel.GetById(lmsCompany.LanguageId).TwoLetterCode);
                }
                else
                {
                    Logger.ErrorFormat("Adobe Connect integration is not set up. param:{0}.",
                        JsonConvert.SerializeObject(param, Formatting.Indented));
                    throw new LtiException(Resources.Messages.LtiValidationNoSetup);
                }

                if (!lmsCompany.IsActive)
                {
                    Logger.ErrorFormat("LMS license is not active. Request's lms_domain:{0}. oauth_consumer_key:{1}.", param.lms_domain, param.oauth_consumer_key);
                    throw new LtiException(Resources.Messages.LtiValidationInactiveLmsLicense);
                }

                if (!BltiProviderHelper.VerifyBltiRequest(lmsCompany, Request, () => true))
                {
                    Logger.ErrorFormat("Invalid LTI request. Invalid signature. oauth_consumer_key:{0}.",
                        param.oauth_consumer_key);
                    throw new LtiException(Resources.Messages.LtiValidationWrongSignature);
                }

                ValidateLtiVersion(param);

                ValidateIntegrationRequiredParameters(lmsCompany, param);

                var contextRoles = GetContextRoles(param.roles);
                if(!contextRoles.Any())
                    throw new LtiException(Resources.Messages.LtiValidationNoContextRole);

                var adobeConnectProvider = this.GetAdminProvider(lmsCompany);

                // TRICK: if LMS don't return user login - try to call lms' API to fetch user's info using user's LMS-ID.
                param.ext_user_username = usersSetup.GetParamLogin(param, lmsCompany); // NOTE: is saved in session!

                var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                LmsUserSession session = this.SaveSession(lmsCompany, param, lmsUser);
                var key = session.Id.ToString();
                if (!string.IsNullOrWhiteSpace(param.lis_outcome_service_url) &&
                    !string.IsNullOrWhiteSpace(param.lis_result_sourcedid))
                {
                    return View("~/Views/Lti/Outcomes.cshtml", new OutcomeModel {LmsProviderName = key});
                }
                //this.meetingSetup.SetupFolders(lmsCompany, adobeConnectProvider);

                Principal acPrincipal = null;

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
                                Username = GetUserNameOrEmail(param),
                                PrincipalId = acPrincipal?.PrincipalId,
                            };
                            this.lmsUserModel.RegisterSave(lmsUser);

                            // TRICK: save lmsUser to session
                            SaveSessionUser(session, lmsUser);
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

                return this.RedirectToExtJs(session, lmsUser, key, null);
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
                    JsonConvert.SerializeObject(param, Formatting.Indented));
                this.ViewBag.Message = ex.Message;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "LoginWithProvider exception. oauth_consumer_key:{0}.", param.oauth_consumer_key);
                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
        }

        //Outcomes: copy-pasted with adding eSync specifics, needed for certification only
        [HttpGet]
        public ActionResult Outcomes(string lmsProviderName)
        {
            if (string.IsNullOrEmpty(lmsProviderName))
            {
                this.ViewBag.Message = "Required parameters were not provided. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            var model = new OutcomeModel
            {
                LmsProviderName = lmsProviderName
            };
            return View("~/Views/Lti/Outcomes.cshtml", model);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Outcomes(OutcomeModel model, string submit)
        {
            if (string.IsNullOrEmpty(model.LmsProviderName))
            {
                this.ViewBag.Message = "Session parameter is required. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }
            var session = GetReadOnlySession(model.LmsProviderName);
            var lmsLicense = session.LmsCompany;
            var outcomeUrl = session.LtiSession.LtiParam.lis_outcome_service_url;
            var outcomeSourceId = session.LtiSession.LtiParam.lis_result_sourcedid;
            if (string.IsNullOrWhiteSpace(outcomeUrl) || string.IsNullOrWhiteSpace(outcomeSourceId))
            {
                this.ViewBag.Message = "Outcomes service is unavailable for current session. Please refresh page and try again.";
                return this.View("~/Views/Lti/LtiError.cshtml");
            }

            switch (submit)
            {
                case "Send Grade":
                    if (LtiOutcomesHelper.PostScore(outcomeUrl, lmsLicense.ConsumerKey, lmsLicense.SharedSecret,
                        outcomeSourceId, model.Score))
                    {
                        ViewBag.Message = "Grade sent";
                    }
                    else
                    {
                        ViewBag.Message = "Invalid request";
                    }
                    break;
                case "Read Grade":
                    var lisResult = LtiOutcomesHelper.ReadScore(outcomeUrl, lmsLicense.ConsumerKey,
                        lmsLicense.SharedSecret, outcomeSourceId);
                    if (lisResult.IsValid)
                    {
                        model.Score = lisResult.Score;
                        ViewBag.Message = "Grade read";
                    }
                    else
                    {
                        ViewBag.Message = "No grade";
                    }
                    break;
                case "Delete Grade":
                    if (LtiOutcomesHelper.DeleteScore(outcomeUrl, lmsLicense.ConsumerKey, lmsLicense.SharedSecret,
                        outcomeSourceId))
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
            return View("~/Views/Lti/Outcomes.cshtml", model);
        }
    }
}