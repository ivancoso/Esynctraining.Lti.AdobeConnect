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
    [LmsAuthorizeBase]
    [EnableCors(origins: "*", headers: "*", methods: "GET,POST,OPTIONS")]
    public class Mp4Controller : BaseController
    {
        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();

        private TaskClient Mp4Client = IoC.Resolve<TaskClient>();

        private IMeetingSetup MeetingSetup = IoC.Resolve<IMeetingSetup>();


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
        [LmsAuthorizeBase]
        public virtual async Task<OperationResult> Convert(RecordingActionRequestDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            try
            {
                string licenseKey = LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");
                
                return await Mp4ApiUtility.DoConvert(Mp4Client, 
                    Guid.Parse(licenseKey),
                    MP4Service.Contract.Client.LicenseType.MP4,
                    input.RecordingId.ToString(),
                    Logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("Convert", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("convertWithSubtitles")]
        public virtual async Task<OperationResult> ConvertWithSubtitles(RecordingActionRequestDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            try
            {
                string licenseKey = LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service licence. Contact administrator.");

                return await Mp4ApiUtility.DoConvert(Mp4Client,
                    Guid.Parse(licenseKey),
                    MP4Service.Contract.Client.LicenseType.MP4WithSubtitles,
                    input.RecordingId.ToString(),
                    Logger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertWithSubtitles", ex);
                return OperationResult.Error(errorMessage);
            }
        }


        [HttpPost]
        [Route("file/{scoId:long:min(1)}")]
        public virtual OperationResultWithData<string> AccessMp4File(string scoId)
        {
            try
            {
                var ac = this.GetAdminProvider();
                string breezeToken;
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                OperationResultWithData<string> result = new SubtitleUtility(ac, Logger, this).AccessMp4File(scoId,
                    LmsCompany.AcServer,
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
                Logger.ErrorFormat(ex, "Mp4Video exception. sco-id:{0}. SessionID: {1}.", scoId, Session.Id);

                return OperationResultWithData<string>.Error(IsDebug 
                    ? (ex.Message + ex.StackTrace) 
                    : "An exception is occured. Try again later or contact your administrator.");
            }
        }

        [HttpPost]
        [Route("subtitle/{scoId:long:min(1)}")]
        public OperationResultWithData<string> AccessVttFile(string scoId)
        {
            try
            {
                string breezeToken;
                var ac = GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                return new SubtitleUtility(ac, Logger, this).AccessVttFile(scoId,
                    LmsCompany.AcServer,
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
        public HttpResponseMessage GetVttFile(string fileScoId, [FromUri]string session)
        {
            try
            {
                string breezeToken;
                var ac = this.GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

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
        public HttpResponseMessage GetVttFileViaPost(string fileScoId)
        {
            try
            {
                string breezeToken;
                var ac = this.GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

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
        public Task<FileUploadResultDto> PostVttFile(string fileScoId)
        {
            var ac = GetAdminProvider();
            return new SubtitleUtility(ac, Logger, this).PostVttFile(fileScoId);
        }


        private Principal GetPrincipal(ILmsLicense lmsCompany, DTO.LtiParamDTO param, string scoId,
             IAdobeConnectProxy provider, out string breezeToken)
        {
            breezeToken = string.Empty;
            
            var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
            if (lmsUser == null)
            {
                throw new WarningMessageException(string.Format("No user with id {0} found in the database.",
                    param.lms_user_id));
            }

            var loginResult = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, provider);
            breezeToken = loginResult.BreezeSession;
            return loginResult.User;
        }

    }

}
