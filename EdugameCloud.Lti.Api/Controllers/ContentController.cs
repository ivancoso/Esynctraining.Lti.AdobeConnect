using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Resources;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.Api;
using Esynctraining.AdobeConnect.Api.Content.Controllers;
using Esynctraining.AdobeConnect.Api.Content.Dto;
using Esynctraining.Core;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("my-content")]
    public class ContentController : BaseApiController
    {
        private readonly LmsUserModel _lmsUserModel;


        private MeetingSetup MeetingSetup => IoC.Resolve<MeetingSetup>();


        public ContentController(
            LmsUserModel lmsUserModel,
            EdugameCloud.Lti.API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger,
            ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
            _lmsUserModel = lmsUserModel;
        }


        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("shortcuts")]
        public OperationResultWithData<IEnumerable<ScoShortcutDto>> GetShortcuts()
        {
            try
            {
                var ac = GetUserProvider();
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
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{folderScoId:long:min(1)}")]
        public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> FolderContent(string folderScoId)
        {
            try
            {
                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());
                return await helper.GetFolderContent(folderScoId, new NullDtoProcessor<ScoContentDto>());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-FolderContent", ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Creates child folder.
        /// </summary>
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{folderScoId:long:min(1)}/create-sub-folder")]
        public OperationResultWithData<FolderDto> CreateSubFolder(string folderScoId, [FromBody]FolderDto dto)
        {
            try
            {
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
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/download")]
        public OperationResult GetDownloadLink(string scoId)
        {
            try
            {
                var ac = this.GetUserProvider();
                var contentService = new ContentService(Logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(Logger, contentService, new ScoContentDtoMapper());

                var param = Session.LtiSession.LtiParam;
                var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, LmsCompany.Id).Value;
                if (lmsUser == null)
                {
                    throw new Core.WarningMessageException($"No user with id {param.lms_user_id} found in the database.");
                }

                string breezeToken = MeetingSetup.ACLogin(LmsCompany, param, lmsUser, ac).BreezeSession;

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
        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/delete")]
        public OperationResult DeleteFileOrFolder(string scoId)
        {
            try
            {
                var ac = this.GetUserProvider();
                var helper = new ContentEditControllerHelper(Logger, ac);
                return helper.DeleteSco(scoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-DeleteFileOrFolder", ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/edit")]
        public OperationResult EditSco(string scoId, [FromBody]FileUpdateDto dto)
        {
            try
            {
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

        [TeacherOnly(FeatureName = LmsCompanySettingNames.EnableMyContent)]
        [HttpPost]
        [Route("content/{scoId:long:min(1)}/move-to/{destinationFolderScoId}")]
        public OperationResult MoveFileOrFolder(string scoId, string destinationFolderScoId)
        {
            try
            {
                var ac = this.GetUserProvider();
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
        public OperationResultWithData<ScoContentDto> UploadFile(string folderScoId, IFormFile file)
        {
            // TRICK:
            var form = this.ControllerContext.HttpContext.Request.Form;
            string sessionKey = form["session"];
            Session = GetReadOnlySession(Guid.Parse(sessionKey));
            if (Session == null)
                return OperationResultWithData<ScoContentDto>.Error(Messages.SessionTimeOut);
            LmsCompany = Session.LmsCompany;
            
            // TODO: check user is teacher + feature is enabled

            string name = form["name"];
            string description = form["description"];
            string customUrl = form["customUrl"];

            try
            {
                string fileName = file.FileName;
                
                var ac = this.GetUserProvider();
                var helper = new ContentEditControllerHelper(Logger, ac);
                int fileSize;
                ScoInfoResult createdFile = helper.UploadFile(folderScoId, name, description, customUrl, fileName, file.ContentType, file.OpenReadStream(), out fileSize);

                var sco = ac.GetScoContent(createdFile.ScoInfo.ScoId);
                var dto = new ScoContentDtoMapper().Map(sco.ScoContent);
                //TRICK:
                dto.ByteCount = fileSize;

                return dto.ToSuccessResult();
            }
            catch (Exception ex)
            {
                IUserMessageException userError = ex as IUserMessageException;
                if (userError != null)
                {
                    return OperationResultWithData<ScoContentDto>.Error(ex.Message);
                }

                throw;
            }
        }


        protected LmsUserSession GetReadOnlySession(Guid key)
        {
            var session = IoC.Resolve<LmsUserSessionModel>().GetByIdWithRelated(key).Value;
            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                return null;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(IoC.Resolve<LanguageModel>().GetById(session.LmsCompany.LanguageId).TwoLetterCode);
            return session;
        }

    }

}
