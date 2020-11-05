using System;
using System.IO;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.Content.Dto;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Api.Content.Controllers
{
    public sealed class ContentEditControllerHelper
    {
        private readonly ILogger _logger;
        private readonly IAdobeConnectProxy _acProxy;


        public ContentEditControllerHelper(ILogger logger, IAdobeConnectProxy acProxy)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (acProxy == null)
                throw new ArgumentNullException(nameof(acProxy));

            _logger = logger;
            _acProxy = acProxy;
        }


        public OperationResultWithData<FolderDto> CreateFolder(FolderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                // TODO: validation!! via FluentValidation
                //if (string.IsNullOrWhiteSpace(dto.FolderId))
                //    throw new ArgumentException("folderScoId can't be empty");

                ScoInfoResult updatedSco =
                                _acProxy.CreateSco(
                                    new FolderUpdateItem
                                    {
                                        Name = dto.Name,
                                        Description = dto.Description,
                                        FolderId = dto.FolderId,
                                        Type = ScoType.folder,
                                    });

                if (!updatedSco.Success)
                {
                    if (updatedSco.Status.Code == StatusCodes.no_access &&
                        updatedSco.Status.SubCode == StatusSubCodes.denied)
                    {
                        return OperationResultWithData<FolderDto>.Error(Resources.Messages.AccessDenied);
                    }

                    if (updatedSco.Status.Code == StatusCodes.invalid &&
                        updatedSco.Status.SubCode == StatusSubCodes.duplicate
                        && updatedSco.Status.InvalidField == "name")
                    {
                        return OperationResultWithData<FolderDto>.Error(Resources.Messages.NameNotUnique);
                    }

                    _logger.Error(updatedSco.Status.GetErrorInfo());
                    return OperationResultWithData<FolderDto>.Error(updatedSco.Status.GetErrorInfo());
                }

                dto.ScoId = updatedSco.ScoInfo.ScoId;
                return dto.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateFolder", ex);
                return OperationResultWithData<FolderDto>.Error(errorMessage);
            }
        }

        public OperationResult MoveSco(string scoId, string destinationFolderScoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            if (string.IsNullOrWhiteSpace(destinationFolderScoId))
                throw new ArgumentException("Non-empty value expected", nameof(destinationFolderScoId));

            try
            {
                //var scoResult = _acProxy.GetScoInfo(scoId);
                //ScoType scoType = scoResult.ScoInfo.Type;
                // TODO: validation!! via FluentValidation
                //if (string.IsNullOrWhiteSpace(dto.FolderId))
                //    throw new ArgumentException("folderScoId can't be empty");
                StatusInfo result = _acProxy.MoveSco(destinationFolderScoId, scoId);

                if (result.Code != StatusCodes.ok)
                {
                    return OperationResult.Error(result.GetErrorInfo());
                }
                return OperationResult.Success();
            }
            catch (AdobeConnectException ex)
            {
                if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
                {
                    return OperationResult.Error(Resources.Messages.AccessDenied);
                }
                if (ex.Status.Code == StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.duplicate
                    && ex.Status.InvalidField == "name")
                {
                    return OperationResult.Error(Resources.Messages.NameNotUnique);
                }

                string errorMessage = GetOutputErrorMessage("DeleteFolder", ex);
                return OperationResult.Error(errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteFolder", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        public OperationResult EditSco(string scoId, FileUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            try
            {
                ScoInfoResult scoResult = _acProxy.GetScoInfo(scoId);
                if (!scoResult.Success)
                {
                    if (scoResult.Status.Code == StatusCodes.no_access && scoResult.Status.SubCode == StatusSubCodes.denied)
                    {
                        return OperationResult.Error(Resources.Messages.AccessDenied);
                    }
                    throw new AdobeConnectException(scoResult.Status);
                }

                ScoInfoResult updatedSco =
                    _acProxy.UpdateSco(
                        new FolderUpdateItem
                        {
                            Name = dto.Name,
                            Description = dto.Description,
                            ScoId = scoId,
                            Type = scoResult.ScoInfo.Type,
                        });

                if (!updatedSco.Success)
                {
                    return OperationResult.Error(updatedSco.Status.GetErrorInfo());
                }
                return OperationResult.Success();
            }
            catch (AdobeConnectException ex)
            {
                if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
                {
                    return OperationResult.Error(Resources.Messages.AccessDenied);
                }

                string errorMessage = GetOutputErrorMessage("EditSco", ex);
                return OperationResult.Error(errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditSco", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        /// <summary>
        /// Deletes passed folder or file.
        /// </summary>
        public OperationResult DeleteSco(string scoId)
        {
            if (string.IsNullOrWhiteSpace(scoId))
                throw new ArgumentException("Non-empty value expected", nameof(scoId));

            try
            {
                StatusInfo deleteResult = _acProxy.DeleteSco(scoId);
                if (deleteResult.Code != StatusCodes.ok)
                {
                    return OperationResult.Error(deleteResult.GetErrorInfo());
                }
                return OperationResult.Success();
            }
            catch (AdobeConnectException ex)
            {
                if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
                {
                    return OperationResult.Error(Resources.Messages.AccessDenied);
                }

                string errorMessage = GetOutputErrorMessage("DeleteFolder", ex);
                return OperationResult.Error(errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteFolder", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        public ScoInfoResult UploadFile(string folderScoId, string name, string description, string customUrl, 
            string fileName, string fileContentType, Stream fileStream,
            out int fileSize)
        {
            try
            {
                var createFile = new ContentUpdateItem
                {
                    FolderId = folderScoId,
                    Name = name,
                    Description = description,
                    UrlPath = customUrl,
                    Type = ScoType.content,
                };

                ScoInfoResult createdFile = _acProxy.CreateSco(createFile);

                if (!createdFile.Success || createdFile.ScoInfo == null)
                {
                    if ((createdFile.Status.SubCode == StatusSubCodes.duplicate) && (createdFile.Status.InvalidField == "name"))
                        throw new WarningMessageException(Resources.Messages.NameNotUnique);

                    if ((createdFile.Status.SubCode == StatusSubCodes.duplicate) && (createdFile.Status.InvalidField == "url-path"))
                        throw new WarningMessageException(Resources.Messages.UrlPathNotUnique);

                    if ((createdFile.Status.SubCode == StatusSubCodes.illegal_operation) && (createdFile.Status.InvalidField == "url-path"))
                        throw new WarningMessageException(Resources.Messages.UrlPathReserved);

                    if ((createdFile.Status.SubCode == StatusSubCodes.format) && (createdFile.Status.InvalidField == "url-path"))
                        throw new WarningMessageException(Resources.Messages.UrlPathInvalidFormat);

                    _logger.Error("UploadFile.CreateSco error. " + createdFile.Status.GetErrorInfo());
                    throw new WarningMessageException(createdFile.Status.Code.ToString() + " " + createdFile.Status.SubCode.ToString());
                }

                var uploadScoInfo = new UploadScoInfo
                {
                    scoId = createdFile.ScoInfo.ScoId,
                    fileContentType = fileContentType,
                    fileName = fileName,
                    fileBytes = fileStream.ReadToEnd(),
                    title = fileName,
                };

                // TRICK:
                fileSize = uploadScoInfo.fileBytes.Length;

                try
                {
                    StatusInfo uploadResult = _acProxy.UploadContent(uploadScoInfo);
                }
                catch (AdobeConnectException ex)
                {
                    try
                    {
                        _acProxy.DeleteSco(createdFile.ScoInfo.ScoId);
                    }
                    catch
                    {
                    }

                    // Status.Code: invalid. Status.SubCode: format. Invalid Field: file
                    if (ex.Status.Code == StatusCodes.invalid && ex.Status.SubCode == StatusSubCodes.format && ex.Status.InvalidField == "file")
                        throw new WarningMessageException(Resources.Messages.FileInvalidFormat);

                    throw new WarningMessageException("Error occured during file uploading.", ex);
                }
                
                return createdFile;

            }
            catch (Exception ex)
            {
                _logger.Error("UploadFile", ex);
                throw;
            }
        }

        private string GetOutputErrorMessage(string methodName, Exception ex)
        {
            // TODO: logger.Error(methodName + lmsInfo, ex);

            if (ex is IUserMessageException)
                return ex.Message;

            return ex.ToString();

            //TODO:
            //return IsDebug
            //    ? Resources.Messages.ExceptionOccured + ex.ToString()
            //    : Resources.Messages.ExceptionMessage;
        }
 
    }

}
