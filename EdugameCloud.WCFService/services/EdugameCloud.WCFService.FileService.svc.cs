// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class FileService : BaseService, IFileService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The file upload end.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<FileDTO> FileUploadEnd(string id)
        {
            var result = new ServiceResponse<FileDTO>();
            Guid idVal;
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out idVal))
            {
                File file = this.FileModel.GetOneByWebOrbId(idVal).Value;
                if (file != null)
                {
                    if (this.FileModel.CompleteFile(file))
                    {
                        result.@object = new FileDTO(file);
                    }
                    else
                    {
                        result.SetError(
                            new Error(
                                Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, 
                                ErrorsTexts.ImageUploadEnd_Subject, 
                                ErrorsTexts.ImageUploadEnd_WebOrbNotFound));
                    }
                }
                else
                {
                    result.SetError(
                        new Error(
                            Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, 
                            ErrorsTexts.ImageUploadEnd_Subject, 
                            ErrorsTexts.ImageUploadFailed_FileNotFound));
                }
            }

            if (result.error != null)
            {
                this.Logger.Error(
                    string.Format(
                        "FileEnd. Error result for weborbId: {0}, Error code: {1}, Error Message: {2}", 
                        id, 
                        result.error.errorCode, 
                        result.error.errorMessage));
            }

            return result;
        }

        /// <summary>
        /// The file upload end.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse FileUploadFailed(string id)
        {
            var result = new ServiceResponse();
            Guid idVal;
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out idVal))
            {
                File file = this.FileModel.GetOneByWebOrbId(idVal).Value;
                if (file != null)
                {
                    this.FileModel.RegisterDelete(file);
                    return result;
                }

                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, 
                        ErrorsTexts.ImageUploadFailed_Subject, 
                        ErrorsTexts.ImageUploadFailed_FileNotFound));
            }

            this.Logger.Error(
                string.Format(
                    "FileEnd. Error result for weborbId: {0}, Error code: {1}, Error Message: {2}", 
                    id, 
                    result.error.errorCode, 
                    result.error.errorMessage));
            return result;
        }

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<string> InitializeFileUpload(FileDTO file)
        {
            User user = null;
            var result = new ServiceResponse<string>();
            ValidationResult validationResult;
            if (this.IsValid(file, out validationResult))
            {
                user = file.createdBy.HasValue ? this.UserModel.GetOneById(file.createdBy.Value).Value : null;
                if (user != null)
                {
                    File createdFile = this.FileModel.CreateFile(user, file.fileName, file.dateCreated, file.width, file.height, file.x, file.y);
                    result.@object = createdFile.Id.ToString();
                }
                else
                {
                    result.SetError(
                        new Error(
                            Errors.CODE_ERRORTYPE_INVALID_USER, 
                            ErrorsTexts.ImageUploadInitialization_Subject, 
                            ErrorsTexts.ImageUploadInitialization_CurrentUserNotFound));
                }
            }

            result = (ServiceResponse<string>)this.UpdateResult(result, validationResult);
            if (result.error != null)
            {
                this.Logger.Error(
                    string.Format(
                        "ImageUploadInitialization. Error result for user: {0}, Error code: {1}, Error Message: {2}", 
                        user.With(x => x.Id), 
                        result.error.errorCode, 
                        result.error.errorMessage));
            }

            return result;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<FileDTO> Save(FileDTO dto)
        {
            ValidationResult result;
            var response = new ServiceResponse<FileDTO>();
            if (this.IsValid(dto, out result))
            {
                FileModel companyModel = this.FileModel;
                File instance = companyModel.GetOneById(dto.fileId).Value;
                if (instance != null)
                {
                    instance = this.ConvertDto(dto, instance);
                    companyModel.RegisterSave(instance, true);
                    response.@object = new FileDTO(instance);
                }
                else
                {
                    response.status = Errors.CODE_RESULTTYPE_ERROR;
                    response.error = new Error(
                        Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, 
                        ErrorsTexts.GetResultError_NotFound, 
                        ErrorsTexts.EntityCreationError_Subject);
                }

                return response;
            }

            response = this.UpdateResult(response, result);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, response, string.Empty);
            return response;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<FileDTO> GetById(Guid id)
        {
            var result = new ServiceResponse<FileDTO>();
            File session;
            if ((session = this.FileModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new FileDTO(session);
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="fileDTO">
        /// The file DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="File"/>.
        /// </returns>
        private File ConvertDto(FileDTO fileDTO, File instance)
        {
            instance.Height = fileDTO.height;
            instance.Width = fileDTO.width;
            instance.X = fileDTO.x;
            instance.Y = fileDTO.y;
            if (!(string.IsNullOrWhiteSpace(fileDTO.fileName) || instance.FileName == fileDTO.fileName))
            {
                instance.FileName = fileDTO.fileName;
            }

            return instance;
        }

        #endregion
    }
}