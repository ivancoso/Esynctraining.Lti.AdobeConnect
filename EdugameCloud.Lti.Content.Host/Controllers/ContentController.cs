﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Content.Host.Dto;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.AdobeConnect.WebApi;
using Esynctraining.AdobeConnect.WebApi.Content.Controllers;
using Esynctraining.AdobeConnect.WebApi.Content.Dto;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.WebApi.Formatting;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Content.Host.Controllers
{

    [RoutePrefix("")]
    [EnableCors(origins: "*", headers: "*", methods: "post,put,delete")] //POST,OPTIONS
    public class ContentController : BaseController
    {
        private readonly LmsUserModel _lmsUserModel;


        private MeetingSetup MeetingSetup
        {
            get { return IoC.Resolve<MeetingSetup>(); }
        }

        private LmsUserModel LmsUserModel
        {
            get { return IoC.Resolve<LmsUserModel>(); }
        }

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
        [Route("{lmsProviderName:guid}/shortcuts")]
        public OperationResultWithData<IEnumerable<ScoShortcutDto>> GetShortcuts(string lmsProviderName)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var ac = this.GetAdobeConnectProvider(lmsCompany);

                var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

                var principalInfo = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo;

                var contentService = new ContentService(logger, ac);

                ScoShortcut sharedContent = contentService.GetShortcuts(new ScoShortcutType[] { ScoShortcutType.content }).First();
                ScoContent userFolder = contentService.GetUserContentFolder(principalInfo.Principal.Login ?? principalInfo.Principal.Email);

                var result = new List<ScoShortcutDto>();
                result.Add(new ScoShortcutDto { ScoId = sharedContent.ScoId, Type = ScoShortcutType.content.ToString() });
                result.Add(new ScoShortcutDto { ScoId = userFolder.ScoId, Type = ScoShortcutType.user_content.ToString() });

                return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Success(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-GetShortcuts", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoShortcutDto>>.Error(errorMessage);
            }
        }

        ///// <summary>
        ///// Returns current teacher's "My Content" folder's root objects.
        ///// </summary>
        //[HttpPost]
        //[Route("{lmsProviderName:guid}/my-content")]
        //public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> MyContent(string lmsProviderName)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var param = session.LtiSession.LtiParam;
        //        var ac = this.GetAdobeConnectProvider(lmsCompany);

        //        var lmsUser = _lmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;

        //        var principalInfo = ac.GetOneByPrincipalId(lmsUser.PrincipalId).PrincipalInfo;

        //        var contentService = new ContentService(logger, ac);
        //        var helper = new ContentControllerHelper<ScoContentDto>(logger, contentService, new ScoContentDtoMapper());
        //        return await helper.GetUserContent(principalInfo.Principal.Login ?? principalInfo.Principal.Email);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("ContentApi-MyContent", lmsCompany, ex);
        //        return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
        //    }
        //}

        ///// <summary>
        ///// Returns "Shared Content" folder's root objects.
        ///// </summary>
        //[HttpPost]
        //[Route("{lmsProviderName:guid}/shared-content")]
        //public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> SharedContent(string lmsProviderName)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var ac = this.GetAdobeConnectProvider(lmsCompany);
        //        var contentService = new ContentService(logger, ac);
        //        var helper = new ContentControllerHelper<ScoContentDto>(logger, contentService, new ScoContentDtoMapper());
        //        return await helper.GetSharedContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("ContentApi-SharedContent", lmsCompany, ex);
        //        return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
        //    }
        //}

        /// <summary>
        /// Returns folder's content.
        /// </summary>
        [HttpPost]
        [Route("{lmsProviderName:guid}/content/{folderScoId:long:min(1)}")]
        public async Task<OperationResultWithData<IEnumerable<ScoContentDto>>> FolderContent(string lmsProviderName, string folderScoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(lmsCompany);
                var contentService = new ContentService(logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(logger, contentService, new ScoContentDtoMapper());
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
        [Route("{lmsProviderName:guid}/content/{folderScoId:long:min(1)}/create-sub-folder")]
        public OperationResultWithData<FolderDto> CreateSubFolder(string lmsProviderName, string folderScoId, [FromBody]FolderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            LmsCompany lmsCompany = null;
            try
            {
                // TRICK:
                dto.FolderId = folderScoId;

                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(lmsCompany);
                var helper = new ContentEditControllerHelper(logger, ac);
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
        [Route("{lmsProviderName:guid}/content/{scoId:long:min(1)}/download")]
        public OperationResult GetDownloadLink(string lmsProviderName, string scoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(lmsCompany);
                var contentService = new ContentService(logger, ac);
                var helper = new ContentControllerHelper<ScoContentDto>(logger, contentService, new ScoContentDtoMapper());

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
                string errorMessage = GetOutputErrorMessage("ContentApi-DeleteFileOrFolder", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        /// <summary>
        /// Deletes folder of file.
        /// </summary>
        [HttpPost]
        [Route("{lmsProviderName:guid}/content/{scoId:long:min(1)}/delete")]
        public OperationResult DeleteFileOrFolder(string lmsProviderName, string scoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(lmsCompany);
                var contentService = new ContentService(logger, ac);
                var helper = new ContentEditControllerHelper(logger, ac);
                return helper.DeleteSco(scoId);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ContentApi-DeleteFileOrFolder", lmsCompany, ex);
                return OperationResultWithData<IEnumerable<ScoContentDto>>.Error(errorMessage);
            }
        }

        [HttpPost]
        [Route("{lmsProviderName:guid}/content/{scoId:long:min(1)}/move-to/{destinationFolderScoId}")]
        public OperationResult MoveFileOrFolder(string lmsProviderName, string scoId, string destinationFolderScoId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var ac = this.GetAdobeConnectProvider(lmsCompany);
                var contentService = new ContentService(logger, ac);
                var helper = new ContentEditControllerHelper(logger, ac);
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
        [Route("uploading/{lmsProviderName:guid}/content/{folderScoId:long:min(1)}/upload-file")]
        public async Task<HttpResponseMessage> UploadFile(string lmsProviderName, string folderScoId)
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
            
            string fileName = provider.FileStreams.First().Key;
            MultipartFormDataMemoryStreamProvider.FileContent stream = provider.FileStreams.First().Value;

            var createFile = new ContentUpdateItem
            {
                FolderId = folderScoId,
                Name = name,
                Description = description,
                UrlPath = customUrl,
                Type = ScoType.content,
            };

            var session = GetReadOnlySession(lmsProviderName);
            lmsCompany = session.LmsCompany;
            var ac = this.GetAdobeConnectProvider(lmsCompany);
            // TODO: check result; unique name\url!
            ScoInfoResult createdFile = ac.CreateSco(createFile);

            var uploadScoInfo = new UploadScoInfo
            {
                scoId = createdFile.ScoInfo.ScoId,
                fileContentType = stream.ContentType,
                fileName = fileName,
                fileBytes = stream.Stream.ReadToEnd(),
                title = fileName,
            };
            StatusInfo uploadResult = ac.UploadContent(uploadScoInfo);

            var sco = ac.GetScoContent(createdFile.ScoInfo.ScoId);
            var dto = new ScoContentDtoMapper().Map(sco.ScoContent);

            //TRICK:
            dto.ByteCount = uploadScoInfo.fileBytes.Length;
            

            string output = JsonConvert.SerializeObject(OperationResultWithData<ScoContentDto>.Success(dto));
            var response = new HttpResponseMessage();
            response.Content = new StringContent(output);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        //[HttpGet]
        //[Route("uploading-test")]
        //public async Task<HttpResponseMessage> Test()
        //{
        //    string output = JsonConvert.SerializeObject(OperationResultWithData<ScoContentDto>.Success(new ScoContentDto { }));
        //    //return Request.CreateResponse(HttpStatusCode.OK, output, "text/html");
        //    var response = new HttpResponseMessage();
        //    response.Content = new StringContent(output);
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        //    return response;
        //}

    }

}
