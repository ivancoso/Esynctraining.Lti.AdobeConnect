using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Api.Controllers;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Mp4.Host.Dto;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.Mp4Service.Tasks.Client.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    [Route("mp4")]
    public class Mp4Controller : BaseApiController
    {
        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();

        private TaskClient Mp4Client = IoC.Resolve<TaskClient>();

        private IMeetingSetup MeetingSetup = IoC.Resolve<IMeetingSetup>();


        public Mp4Controller(
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
        }


        [HttpPost]
        [Route("convert")]
        [LmsAuthorizeBase]
        public virtual async Task<OperationResult> Convert([FromBody]RecordingActionRequestDto input)
        {
            try
            {
                string licenseKey = LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service license. Contact administrator.");

                Guid license;
                if (!Guid.TryParse(licenseKey, out license))
                    throw new WarningMessageException("Invalid MP4Service license. Contact administrator.");

                return await Mp4ApiUtility.DoConvert(Mp4Client,
                    license,
                    MP4Service.Contract.Client.LicenseType.MP4,
                    input.RecordingId,
                    null,
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
        [LmsAuthorizeBase]
        public virtual async Task<OperationResult> ConvertWithSubtitles([FromBody]RecordingActionRequestDto input)
        {
            try
            {
                string licenseKey = LmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (string.IsNullOrWhiteSpace(licenseKey))
                    throw new WarningMessageException("Can't find your MP4Service license. Contact administrator.");

                Guid license;
                if (!Guid.TryParse(licenseKey, out license))
                    throw new WarningMessageException("Invalid MP4Service license. Contact administrator.");

                return await Mp4ApiUtility.DoConvert(Mp4Client,
                    license,
                    MP4Service.Contract.Client.LicenseType.MP4WithSubtitles,
                    input.RecordingId,
                    null,
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
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<string> AccessMp4File(string scoId)
        {
            try
            {
                var ac = this.GetAdminProvider();
                string breezeToken;
                Principal principal = GetPrincipal(this.LmsCompany, Session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                OperationResultWithData<string> result = new SubtitleUtility(ac, Logger).AccessMp4File(scoId,
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
        [LmsAuthorizeBase]
        public OperationResultWithData<string> AccessVttFile(string scoId)
        {
            try
            {
                string breezeToken;
                var ac = GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, scoId, ac, out breezeToken);

                return new SubtitleUtility(ac, Logger).AccessVttFile(scoId,
                    LmsCompany.AcServer,
                    principal.PrincipalId,
                    breezeToken);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("AccessVttFile", ex);
                return OperationResultWithData<string>.Error(errorMessage);
            }
        }

        [HttpGet]
        [Route("subtitle/{fileScoId:long:min(1)}")]
        [QueryStringLmsAuthorize]
        public ActionResult GetVttFile(string fileScoId)
        {
            try
            {
                string breezeToken;
                var ac = GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

                //return new SubtitleUtility(ac, Logger).GetVttFile(principal.PrincipalId, fileScoId);
                ScoInfo sco = DoGetSco(fileScoId, ac, principal.PrincipalId);
                FileEntry file = GetOriginalFileContent(sco, ac);
                if (file == null)
                {
                    return NotFound();
                }
                //string contentType;
                //new FileExtensionContentTypeProvider().TryGetContentType(FileName, out contentType);
                //return contentType ?? "application/octet-stream";

                return File(file.Content, "text/html", file.FileName);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVttFile", ex);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("subtitle/{fileScoId:long:min(1)}/content")]
        [LmsAuthorizeBase]
        public ActionResult GetVttFileViaPost(string fileScoId)
        {
            try
            {
                string breezeToken;
                var ac = this.GetAdminProvider();
                Principal principal = GetPrincipal(LmsCompany, Session.LtiSession.LtiParam, fileScoId, ac, out breezeToken);

                //return new SubtitleUtility(ac, Logger).GetVttFile(principal.PrincipalId, fileScoId);

                ScoInfo sco = DoGetSco(fileScoId, ac, principal.PrincipalId);
                FileEntry file = GetOriginalFileContent(sco, ac);
                if (file == null)
                {
                    return NotFound();
                }
                //string contentType;
                //new FileExtensionContentTypeProvider().TryGetContentType(FileName, out contentType);
                //return contentType ?? "application/octet-stream";

                return File(file.Content, "text/html", file.FileName);
            }
            catch (Exception ex)
            {
                Logger.Error("GetVttFile", ex);
                return StatusCode(500);
            }
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("subtitle/{fileScoId:long:min(1)}/content/save")]
        [LmsAuthorizeBase]
        public object PostVttFile(string fileScoId, [FromForm]IFormFile file)
        {
            if (file == null)
                return new //FileUploadResultDto
                {
                    IsSuccess = false,
                    Message = "No file uploaded",
                };

            var ac = GetAdminProvider();
            //return new SubtitleUtility(ac, Logger, this).PostVttFile(fileScoId);

            FileDto acFile = Create(fileScoId,
                file.FileName,
                file.ContentType,
                file.OpenReadStream().ReadFully(),
                ac);

            return new //FileUploadResultDto
            {
                IsSuccess = true,
                Message = "OK",
                Result = new FileDescription
                {
                    FileId = acFile.Id,
                    FileName = acFile.Name,
                },
            };
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

        private ScoInfo DoGetSco(string scoId, IAdobeConnectProxy ac, string principalId)
        {
            // check is user already has read permission!!!
            // TODO: setup only if source recording is accessible??
            //  ac.UpdateScoPermissionForPrincipal(scoId, principalId, MeetingPermissionId.view);
            var sco = ac.GetScoInfo(scoId);
            if (sco.Status.Code == Esynctraining.AC.Provider.Entities.StatusCodes.no_access && sco.Status.SubCode == StatusSubCodes.denied)
            {
                Logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                throw new WarningMessageException("Access denied.");
            }
            if (sco.Status.Code == Esynctraining.AC.Provider.Entities.StatusCodes.no_data && sco.Status.SubCode == StatusSubCodes.not_set)
            {
                Logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                throw new WarningMessageException("File not found in Adobe Connect.");
            }
            else if (!sco.Success)
            {
                Logger.ErrorFormat("DoGetSco: {0}. sco-id:{1}.", sco.Status.GetErrorInfo(), scoId);
                string msg = string.Format("[AdobeConnectProxy Error] {0}. Parameter1:{1}.",
                 sco.Status.GetErrorInfo(),
                 scoId);
                throw new InvalidOperationException(msg);
            }

            return sco.ScoInfo;
        }

        private static FileDto Create(string fileScoId, string fileName, string fileContentType, byte[] content, IAdobeConnectProxy ac)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName can't be empty", nameof(fileName));
            if (string.IsNullOrWhiteSpace(fileContentType))
                throw new ArgumentException("fileContentType can't be empty", nameof(fileContentType));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var uploadScoInfo = new UploadScoInfo
            {
                scoId = fileScoId,
                fileContentType = fileContentType,
                fileName = fileName,
                fileBytes = content,
                title = fileName,
            };

            try
            {
                string originalFileName = fileName;
                StatusInfo uploadResult = ac.UploadContent(uploadScoInfo);
            }
            catch (AdobeConnectException ex)
            {
                // Status.Code: invalid. Status.SubCode: format. Invalid Field: file
                if (ex.Status.Code == Esynctraining.AC.Provider.Entities.StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.format && ex.Status.InvalidField == "file")
                    throw new Exception("Invalid file format selected.");

                throw new Exception("Error occured during file uploading.", ex);
            }

            return new FileDto
            {
                Id = fileScoId,
                Name = fileName,
                Size = content.Length,
            };
        }

        internal sealed class FileEntry
        {
            public string FileName { get; set; }

            public byte[] Content { get; set; }


            public FileEntry(byte[] content, string fileName)
            {
                Content = content;
                FileName = fileName;
            }

        }

        private static FileEntry GetOriginalFileContent(ScoInfo file, IAdobeConnectProxy provider)
        {
            string error;
            byte[] content = provider.GetContentByUrlPath(file.UrlPath, "zip", out error);

            var archive = new ZipArchive(new MemoryStream(content));
            ZipArchiveEntry fileEntry = archive.Entries[0];

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                fileEntry.Open().CopyTo(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            return new FileEntry(fileContent, fileEntry.Name);
        }


        //....

        // TRICK: for upload only
        private LanguageModel LanguageModel => IoC.Resolve<LanguageModel>();

        private LmsUserSessionModel UserSessionModel => IoC.Resolve<LmsUserSessionModel>();

        protected LmsUserSession GetReadOnlySession(Guid key)
        {
            var session = UserSessionModel.GetByIdWithRelated(key).Value;
            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);
            return session;
        }

    }

}
