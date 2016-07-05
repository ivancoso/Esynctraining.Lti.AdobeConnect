using System;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.WebApi.Content.Dto;
using Esynctraining.Core;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.WebApi.Content.Controllers
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
                    _logger.Error(updatedSco.Status.GetErrorInfo());
                    return OperationResultWithData<FolderDto>.Error(updatedSco.Status.GetErrorInfo());
                }

                dto.ScoId = updatedSco.ScoInfo.ScoId;
                return OperationResultWithData<FolderDto>.Success(dto);
            }
            catch (AdobeConnectException ex)
            {
                if (ex.Status.Code == StatusCodes.no_access && ex.Status.SubCode == StatusSubCodes.denied)
                {
                    return OperationResultWithData<FolderDto>.Error("You do not have permission to access this item.");
                }

                string errorMessage = GetOutputErrorMessage("CreateFolder", ex);
                return OperationResultWithData<FolderDto>.Error(errorMessage);
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
                    return OperationResult.Error("You do not have permission to access this item.");
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
                        return OperationResult.Error("You do not have permission to access this item.");
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
                    return OperationResult.Error("You do not have permission to access this item.");
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
                    return OperationResult.Error("You do not have permission to access this item.");
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
