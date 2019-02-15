//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using System.Web.UI;
//using EdugameCloud.Lti.API.AdobeConnect;
//using EdugameCloud.Lti.Core;
//using EdugameCloud.Lti.Core.Business.Models;
//using EdugameCloud.Lti.Core.Constants;
//using EdugameCloud.Lti.Core.DTO;
//using EdugameCloud.Lti.Domain.Entities;
//using EdugameCloud.Lti.DTO;
//using EdugameCloud.Lti.Extensions;
//using Esynctraining.Core.Domain;
//using Esynctraining.Core.Logging;
//using Esynctraining.Core.Providers;
//using Esynctraining.Core.Utils;
//using Esynctraining.Mp4Service.Tasks.Client;
//using Esynctraining.WebApi.Client;

//namespace EdugameCloud.Lti.Controllers
//{
//    public partial class Mp4ServiceController : BaseController
//    {
//        public Mp4ServiceController(
//            LmsUserSessionModel userSessionModel,
//            IAdobeConnectAccountService acAccountService,
//            ApplicationSettingsProvider settings,
//            ILogger logger)
//            : base(userSessionModel, acAccountService, settings, logger)
//        {
//        }


//        [HttpPost]
//        public virtual async Task<JsonResult> Convert(string lmsProviderName, string recordingId)
//        {
//            LmsCompany lmsCompany = null;
//            try
//            {
//                var session = GetReadOnlySession(lmsProviderName);
//                lmsCompany = session.LmsCompany;

//                string licenseKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Mp4ServiceLicenseKey);
//                if (string.IsNullOrWhiteSpace(licenseKey))
//                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

//                var mp4Client = IoC.Resolve<Mp4ServiceTaskClient>();
//                MP4Service.Contract.Client.DataTask task = null;
//                try
//                {
//                    var license = await mp4Client.GetLicense(new Guid(licenseKey)).ConfigureAwait(false);
//                    if (license.Type != MP4Service.Contract.Client.LicenseType.MP4)
//                        throw new WarningMessageException("Invalid MP4 license type");

//                    task = await mp4Client.Convert(new MP4Service.Contract.Client.TaskParam
//                    {
//                        LicenseId = licenseKey,
//                        ScoId = long.Parse(recordingId),
//                    }).ConfigureAwait(false);
//                }
//                catch (AggregateException ex)
//                {
//                    return Json(ProcessAggregateException(ex, "Message from MP4 API: "));
//                }
//                catch (ApiException ex)
//                {
//                    return Json(ProcessApiException(ex, "Message from MP4 API: "));
//                }


//                return Json(OperationResultWithData<string>.Success(task.Status.ToString()));
//            }
//            catch (Exception ex)
//            {
//                string errorMessage = GetOutputErrorMessage("Convert", lmsCompany, ex);
//                return Json(OperationResult.Error(errorMessage));
//            }
//        }

//        [HttpPost]
//        public virtual async Task<JsonResult> ConvertWithSubtitles(string lmsProviderName, string recordingId)
//        {
//            LmsCompany lmsCompany = null;
//            try
//            {
//                var session = GetReadOnlySession(lmsProviderName);
//                lmsCompany = session.LmsCompany;

//                string licenseKey = lmsCompany.GetSetting<string>(LmsLicenseSettingNames.Mp4ServiceWithSubtitlesLicenseKey);
//                if (string.IsNullOrWhiteSpace(licenseKey))
//                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

//                var mp4Client = IoC.Resolve<Mp4ServiceTaskClient>();
//                MP4Service.Contract.Client.DataTask task = null;
//                try
//                {
//                    var license = await mp4Client.GetLicense(new Guid(licenseKey)).ConfigureAwait(false);
//                    if (license.Type != MP4Service.Contract.Client.LicenseType.MP4WithSubtitles)
//                        throw new WarningMessageException("Invalid MP4 license type");

//                    task = await mp4Client.Convert(new MP4Service.Contract.Client.TaskParam
//                    {
//                        LicenseId = licenseKey,
//                        ScoId = long.Parse(recordingId),
//                    }).ConfigureAwait(false);
//                }
//                catch (AggregateException ex)
//                {
//                    return Json(ProcessAggregateException(ex, "Message from MP4 API: "));
//                }
//                catch (ApiException ex)
//                {
//                    return Json(ProcessApiException(ex, "Message from MP4 API: "));
//                }

//                return Json(OperationResultWithData<string>.Success(task.Status.ToString()));
//            }
//            catch (Exception ex)
//            {
//                string errorMessage = GetOutputErrorMessage("ConvertWithSubtitles", lmsCompany, ex);
//                return Json(OperationResult.Error(errorMessage));
//            }
//        }


//        [HttpPost]
//        public virtual ActionResult AccessFile(string lmsProviderName, string scoId)
//        {
//            LmsCompany lmsCompany = null;
//            try
//            {
//                var session = GetReadOnlySession(lmsProviderName);
//                lmsCompany = session.LmsCompany;

//                string url = AccessSco(lmsCompany, session.LtiSession.LtiParam, scoId, this.GetAdobeConnectProvider(lmsCompany));

//                //return this.Redirect(url);
//                return Json(OperationResultWithData<string>.Success(url));
//            }
//            catch (WarningMessageException ex)
//            {
//                this.ViewBag.Message = ex.Message;
//                // TRICK: to increase window height
//                this.ViewBag.DebugError = "eSyncTraining Inc.";
//                return this.View("~/Views/Lti/LtiError.cshtml");
//            }
//            catch (Exception ex)
//            {
//                logger.ErrorFormat(ex, "Mp4Video exception. sco-id:{0}. SessionID: {1}.", scoId, lmsProviderName);
//                this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
//                return this.View("~/Views/Lti/LtiError.cshtml");
//            }
//        }


//        private string AccessSco(LmsCompany lmsCompany, LtiParamDTO param, string scoId,
//             IAdobeConnectProxy adobeConnectProvider)
//        {
//            var breezeToken = string.Empty;

//            IAdobeConnectProxy provider = adobeConnectProvider;

//            LmsUserModel lmsUserModel = IoC.Resolve<LmsUserModel>();
//            var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
//            if (lmsUser == null)
//            {
//                throw new WarningMessageException(string.Format("No user with id {0} found in the database.",
//                    param.lms_user_id));
//            }

//            var principalInfo = lmsUser.PrincipalId != null
//                ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo
//                : null;
//            var registeredUser = principalInfo != null ? principalInfo.Principal : null;

//            if (registeredUser != null)
//            {
//                var meetingSetup = IoC.Resolve<IMeetingSetup>();
//                breezeToken = meetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, provider);
//            }
//            else
//            {
//                throw new WarningMessageException(string.Format(
//                    "No user with principal id {0} found in Adobe Connect.", lmsUser.PrincipalId ?? string.Empty));
//            }

//            // check is user already has read permission!!!
//            // TODO: setup only if source recording is accessible??
//            provider.UpdateScoPermissionForPrincipal(scoId, registeredUser.PrincipalId, Esynctraining.AC.Provider.Entities.MeetingPermissionId.view);

//            var baseUrl = lmsCompany.AcServer + provider.GetScoInfo(scoId).ScoInfo.UrlPath;

//            return GetDownloadLink(lmsCompany.AcServer, provider.GetScoInfo(scoId).ScoInfo.UrlPath.Replace("/", ""), "mp4") + "&session=" + breezeToken;
//        }

//        private string GetDownloadLink(string acServer, string downloadName, string format)
//        {
//            string fileName = downloadName.Substring(0, downloadName.Length - 3) + ".mp4";

//            return string.Format(
//                "{0}/{1}/output/{2}?download={2}",
//                acServer.Replace(@"api/xml", string.Empty).Trim('/'),
//                downloadName,
//                fileName);            
//        }

//        protected OperationResult ProcessAggregateException(AggregateException ex, string baseMessage)
//        {
//            foreach (ApiException exception in ex.InnerExceptions.Where(x => x is ApiException))
//            {
//                return ProcessApiException(exception, baseMessage);
//            }

//            foreach (Exception exception in ex.InnerExceptions)
//            {
//                return OperationResult.Error(baseMessage + exception.Message);
//            }

//            return OperationResult.Error(baseMessage + ex.Message);
//        }

//        protected OperationResult ProcessApiException(ApiException ex, string baseMessage)
//        {
//            return OperationResult.Error(baseMessage + (ex.ErrorDetails != null ? ex.ErrorDetails.ToString() : ex.Message));
//        }

//    }

//}