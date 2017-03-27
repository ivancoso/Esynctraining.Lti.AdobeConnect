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
    [EnableCors(origins: "*", headers: "*", methods: "post")]
    public class ContentController : BaseController
    {
        private readonly LmsUserModel _lmsUserModel;


        private IJsonSerializer JsonSerializer => IoC.Resolve<IJsonSerializer>();

        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();

        private LmsUserModel LmsUserModel => IoC.Resolve<LmsUserModel>();


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
        [Route("{session:guid}/shortcuts")]
        public OperationResultWithData<IEnumerable<ScoShortcutDto>> GetShortcuts(string session)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Error("Operation is not enabled.");

                var param = session.LtiSession.LtiParam;
                var ac = this.GetUserProvider(session);

                var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

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
                string errorMessage = GetOutputErrorMessage("ContentApi-GetShortcuts", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Error(errorMessage);
            }
        }
        
        /// <summary>
        /// Returns folder's content.
        /// </summary>
        [HttpPost]
        [Route("{session:guid}/content/{folderScoId:long:min(1)}")]
        public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> FolderContent(string session, string folderScoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<IEnumerable<ScoContentDto>>.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());
                return await helper.GetFolderContent(folderScoId, new NullDtoProcessor<ScoContentDto>());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-FolderContent", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Creates child folder.
        /// </summary>
        [HttpPost]
        [Route("{session:guid}/content/{folderScoId:long:min(1)}/create-sub-folder")]
        public OperationResultWithData<FolderDto> CreateSubFolder(string session, string folderScoId, [FromBody]FolderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            LmsCompany lmsCompany = null;
            try
            {
                // TRICK:
                dto.FolderId = folderScoId;

                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResultWithData<FolderDto>.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.CreateFolder(dto);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-CreateSubFolder", lmsCompany, ex);
                return OperationResultWithData<FolderDto>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Get Download link to download file directly from AC (zip version).
        /// </summary>
        [HttpPost]
        [Route("{session:guid}/content/{scoId:long:min(1)}/download")]
        public OperationResult GetDownloadLink(string session, string scoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());

                var param = session.LtiSession.With(x => x.LtiParam);
                var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                if (lmsUser.PrincipalId == null)
                {
                    throw new Core.WarningMessageException("User doesn't have account in Adobe Connect.");
                }

                var registeredUser = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo.Principal;

                string breezeToken = MeetingSetup.ACLogin(lmsCompany, param, lmsUser, registeredUser, ac);

                return helper.GetDownloadAsZipLink(scoId, breezeToken);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-GetDownloadLink", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Deletes folder of file.
        /// </summary>
        [HttpPost]
        [Route("{session:guid}/content/{scoId:long:min(1)}/delete")]
        public OperationResult DeleteFileOrFolder(string session, string scoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.DeleteSco(scoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-DeleteFileOrFolder", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("{session:guid}/content/{scoId:long:min(1)}/edit")]
        public OperationResult EditSco(string session, string scoId, [FromBody]FileUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.EditSco(scoId, dto);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-EditFile", lmsCompany, ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("{session:guid}/content/{scoId:long:min(1)}/move-to/{destinationFolderScoId}")]
        public OperationResult MoveFileOrFolder(string session, string scoId, string destinationFolderScoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = Session;
                lmsCompany = session.LmsCompany;

                if (!lmsCompany.GetSetting<bool>(LmsCompanySettingNames.EnableMyContent))
                    return OperationResult.Error("Operation is not enabled.");

                var ac = this.GetUserProvider(session);
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.MoveSco(scoId, destinationFolderScoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-MoveFileOrFolder", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("uploading/{session:guid}/content/{folderScoId:long:min(1)}/upload-file")]
        public async Task<HttpResponseMessage> UploadFile(string session, string folderScoId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            LmsCompany lmsCompany = null;

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

                var session = Session;
                lmsCompany = session.LmsCompany;
                var ac = this.GetUserProvider(session);
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
