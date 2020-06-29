using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using EdugameCloud.Core;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Content.Host.Dto;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api;
using Esynctraining.AdobeConnect.Api.Content.Controllers;
using Esynctraining.AdobeConnect.Api.Content.Dto;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.WebApi.Formatting;

namespace EdugameCloud.Lti.Content.Host.Controllers
{
    [RoutePrefix("")]
    [LmsAuthorizeBase]
    [EnableCors(origins: "*", headers: "*", methods: "post")]
    public class ContentController : BaseController
    {
        private readonly LmsUserModel _lmsUserModel;


        private IJsonSerializer JsonSerializer => IoC.Resolve<IJsonSerializer>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();


        public ContentController(
            LmsUserSessionModel userSessionModel,
            Esynctraining.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, LmsUserModel lmsUserModel)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            _lmsUserModel = lmsUserModel;
        }


        [HttpPost]
        [Route("shortcuts")]
        public OperationResultWithData<IEnumerable<ScoShortcutDto>> GetShortcuts()
        {
            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Error("Operation is not enabled.");

                var param = Session.LtiSession.LtiParam;
                var ac = this.GetUserProvider();

                var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, LmsCompany.Id).Value;

                var contentService = new ContentService(Logger, ac);
                IEnumerable<ScoShortcut> shortcuts = contentService.GetShortcuts(new ScoShortcutType[] { ScoShortcutType.content, ScoShortcutType.my_content });

                var result = shortcuts.Select(x => new ScoShortcutDto
                {
                    ScoId = x.ScoId,
                    Type = x.Type,
                    Name = Resources.ScoShortcutNames.ResourceManager.GetString(x.Type.Replace('-', '_')), // TRICK: AC trick to change my-content to my_content
                });

                return result.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-GetShortcuts", ex);
                return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Error(errorMessage);
            }
        }
        
        /// <summary>
        /// Returns folder's content.
        /// </summary>
        [HttpPost]
        [Route("content/{folderScoId:long:min(1)}")]
        public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> FolderContent(string folderScoId)
        {
            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<IEnumerable<ScoContentDto>>.Error("Operation is not enabled.");

                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());
                return await helper.GetFolderContent(folderScoId, new NullDtoProcessor<ScoContentDto>());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-FolderContent",  ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Creates child folder.
        /// </summary>
        [HttpPost]
        [Route("сontent/{folderScoId:long:min(1)}/create-sub-folder")]
        public OperationResultWithData<FolderDto> CreateSubFolder(string folderScoId, [FromBody]FolderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<FolderDto>.Error("Operation is not enabled.");

                // TRICK:
                dto.FolderId = folderScoId;
                
                var ac = this.GetUserProvider();
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.CreateFolder(dto);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-CreateSubFolder", ex);
                return OperationResultWithData<FolderDto>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Get Download link to download file directly from AC (zip version).
        /// </summary>
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/download")]
        public OperationResult GetDownloadLink(string scoId)
        {
            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());

                var param = Session.LtiSession.LtiParam;
                var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, LmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                if (lmsUser.PrincipalId == null)
                {
                    throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                string breezeToken = MeetingSetup.ACLogin(LmsCompany, param, lmsUser, registeredUser, ac);

                return helper.GetDownloadAsZipLink(scoId, breezeToken);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-GetDownloadLink", ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Deletes folder of file.
        /// </summary>
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/delete")]
        public OperationResult DeleteFileOrFolder(string scoId)
        {
            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.DeleteSco(scoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-DeleteFileOrFolder", ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("content/{scoId:long:min(1)}/edit")]
        public OperationResult EditSco(string scoId, [FromBody]FileUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider();
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.EditSco(scoId, dto);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-EditFile", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("content/{scoId:long:min(1)}/move-to/{destinationFolderScoId}")]
        public OperationResult MoveFileOrFolder(string scoId, string destinationFolderScoId)
        {
            try
            {
                if (!LmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.MoveSco(scoId, destinationFolderScoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-MoveFileOrFolder", ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("uploading/content/{folderScoId:long:min(1)}/upload-file")]
        public async Task<HttpResponseMessage> UploadFile(string folderScoId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartFormDataMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            string name = provider.FormData["name"];
            string description = provider.FormData["description"];
            string customUrl = provider.FormData["customUrl"];

            if (!provider.FileStreams.Any())
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);

                //return Content(HttpStatusCode.BadRequest, "No file uploaded");
            }
            if (provider.FileStreams.Count != 1)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
                //return Content(HttpStatusCode.BadRequest, "Single file expected");
            }

            try
            {
                string fileName = provider.FileStreams.First().Key;
                MultipartFormDataMemoryStreamProvider.FileContent stream = provider.FileStreams.First().Value;

                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentEditControllerHelper(Logger, ac);
                int fileSize;
                ScoInfoResult createdFile = helper.UploadFile(folderScoId, name, description, customUrl, fileName, stream.ContentType, stream.Stream, out fileSize);
                
                var sco = ac.GetScoContent(createdFile.ScoInfo.ScoId);
                var dto = new ScoContentDtoMapper().Map(sco.ScoContent);
                //TRICK:
                dto.ByteCount = fileSize;

                string output = JsonSerializer.JsonSerialize(dto.ToSuccessResult());
                var response = new HttpResponseMessage();
                response.Content = new StringContent(output);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
            catch (Exception ex)
            {
                IUserMessageException userError = ex as IUserMessageException;
                if (userError != null)
                {
                    string output = JsonSerializer.JsonSerialize(OperationResultWithData<ScoContentDto>.Error(ex.Message));
                    //JsonConvert.SerializeObject(OperationResultWithData<ScoContentDto>.Error(ex.Message));
                    var response = new HttpResponseMessage();
                    response.Content = new StringContent(output);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    return response;
                }

                throw;
            }
        }

    }

}
