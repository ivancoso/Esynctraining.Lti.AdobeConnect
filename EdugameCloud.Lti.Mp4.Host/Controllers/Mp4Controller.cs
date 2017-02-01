using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Mp4.Host.Dto;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    [RoutePrefix("mp4")]
    [EnableCors(origins: "*", headers: "*", methods: "GET,POST,OPTIONS")]
    public class Mp4Controller : BaseController
    {
        public Mp4Controller(
            LmsUserSessionModel userSessionModel,
            Esynctraining.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
        }
        

        [HttpPost]
        [Route("convert")]
        public virtual async Task<OperationResult> Convert(RecordingActionRequestDto input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(input.LmsProviderName);
                lmsCompany = session.LmsCompany;

                string licenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

                var mp4Client = IoC.Resolve<TaskClient>();
                return await Mp4ApiUtility.DoConvert(mp4Client, 
                    Guid.Parse(licenseKey),
                    MP4Service.Contract.Client.LicenseType.MP4,
                    input.RecordingId.ToString(),
                    Logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Convert", lmsCompany, ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("convertWithSubtitles")]
        public virtual async Task<OperationResult> ConvertWithSubtitles(RecordingActionRequestDto input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(input.LmsProviderName);
                lmsCompany = session.LmsCompany;

                string licenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

                var mp4Client = IoC.Resolve<TaskClient>();
                return await Mp4ApiUtility.DoConvert(mp4Client,
                    Guid.Parse(licenseKey),
                    MP4Service.Contract.Client.LicenseType.MP4WithSubtitles,
                    input.RecordingId.ToString(),
                    Logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertWithSubtitles", lmsCompany, ex);
                return OperationResult.Error(errorMessage);
            }
        }


        [HttpPost]
        [Route("file/{scoId:long:min(1)}")]
        public virtual OperationResultWithData<string> AccessMp4File(string scoId, [FromUri]string lmsProviderName)
        {
            if (lmsProviderName == null)
                throw new ArgumentNullException(nameof(lmsProviderName));

            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                var ac = this.GetAdobeConnectProvider(session);
                string breezeToken;
                Principal principal = GetPrincipal(lmsCompany, session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                OperationResultWithData<string> result = new SubtitleUtility(ac, Logger, this).AccessMp4File(scoId,
                    lmsCompany.AcServer,
                    principal.PrincipalId,
                    breezeToken);

                if (result.IsSuccess)
                    return result;

                return OperationResultWithData<string>.Error(IsDebug
                    ? result.Message
                    : "An exception is occured. Try again later or contact your administrator.");

            }
            catch (WarningMessageException ex)
            {
                return OperationResultWithData<string>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Mp4Video exception. sco-id:{0}. SessionID: {1}.", scoId, lmsProviderName);

                return OperationResultWithData<string>.Error(IsDebug 
                    ? (ex.Message + ex.StackTrace) 
                    : "An exception is occured. Try again later or contact your administrator.");
            }
        }

        [HttpPost]
        [Route("subtitle/{scoId:long:min(1)}")]
        public OperationResultWithData<string> AccessVttFile(string scoId, [FromUri]string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                string breezeToken;
                var ac = this.GetAdobeConnectProvider(session);
                Principal principal = GetPrincipal(lmsCompany, session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                return new SubtitleUtility(ac, Logger, this).AccessVttFile(scoId,
                    lmsCompany.AcServer,
                    principal.PrincipalId,
                    breezeToken);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVttFile", ex);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("subtitle/{fileScoId:long:min(1)}")]
        public HttpResponseMessage GetVttFile(string fileScoId, [FromUri]string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                string breezeToken;
                var ac = this.GetAdobeConnectProvider(session);
                Principal principal = GetPrincipal(lmsCompany, session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

                return new SubtitleUtility(ac, Logger, this).GetVttFile(principal.PrincipalId, fileScoId);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVttFile", ex);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("subtitle/{fileScoId:long:min(1)}/content")]
        public HttpResponseMessage GetVttFileViaPost(string fileScoId, [FromUri]string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                string breezeToken;
                var ac = this.GetAdobeConnectProvider(session);
                Principal principal = GetPrincipal(lmsCompany, session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

                return new SubtitleUtility(ac, Logger, this).GetVttFile(principal.PrincipalId, fileScoId);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVttFile", ex);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("subtitle/{fileScoId:long:min(1)}/content/save")]
        public Task<FileUploadResultDto> PostVttFile(string fileScoId, [FromUri]string lmsProviderName)
        {
            var session = GetReadOnlySession(lmsProviderName);
            var lmsCompany = session.LmsCompany;

            var ac = this.GetAdobeConnectProvider(session);

            return new SubtitleUtility(ac, Logger, this).PostVttFile(fileScoId);
        }

 
        private Principal GetPrincipal(LmsCompany lmsCompany, DTO.LtiParamDTO param, string scoId,
             Esynctraining.AdobeConnect.IAdobeConnectProxy provider, out string breezeToken)
        {
            breezeToken = string.Empty;

            LmsUserModel lmsUserModel = IoC.Resolve<LmsUserModel>();
            var lmsUser = lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                throw new WarningMessageException(string.Format("No user with id {0} found in the database.",
                    param.lms_user_id));
            }

            var principalInfo = lmsUser.PrincipalId != null
                ? provider.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo
                : null;
            var registeredUser = principalInfo != null ? principalInfo.Principal : null;

            if (registeredUser != null)
            {
                var meetingSetup = IoC.Resolve<IMeetingSetup>();
                breezeToken = meetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, provider);
            }
            else
            {
                throw new WarningMessageException(string.Format(
                    "No user with principal id {0} found in Adobe Connect.", lmsUser.PrincipalId ?? string.Empty));
            }

            return registeredUser;
        }

    }

}
