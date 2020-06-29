// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;

    using FluentValidation.Results;

    using Resources;

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
        /// The <see cref="FileDTO"/>.
        /// </returns>
        public FileDTO FileUploadEnd(string id)
        {
            Guid idVal;
            Error error;
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out idVal))
            {
                File file = this.FileModel.GetOneByWebOrbId(idVal).Value;
                if (file != null)
                {
                    if (this.FileModel.CompleteFile(file))
                    {
                        return new FileDTO(file);
                    }

                    error = new Error(
                        Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND,
                        ErrorsTexts.ImageUploadEnd_Subject,
                        ErrorsTexts.ImageUploadEnd_WebOrbNotFound);
                }
                else
                {
                    error =
                        new Error(
                            Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, 
                            ErrorsTexts.ImageUploadEnd_Subject, 
                            ErrorsTexts.ImageUploadFailed_FileNotFound);
                }
            }
            else
            {
                error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.ImageUploadEnd_Subject,
                    "Invalid input");
            }

            this.Logger.Error(
                string.Format(
                    "FileEnd. Error result for weborbId: {0}, Error code: {1}, Error Message: {2}",
                    id,
                    error.errorCode,
                    error.errorMessage));
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The file upload end.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public void FileUploadFailed(string id)
        {
            Guid idVal;
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out idVal))
            {
                File file = this.FileModel.GetOneByWebOrbId(idVal).Value;
                if (file != null)
                {
                    this.FileModel.RegisterDelete(file);
                }

                var error = new Error(Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, ErrorsTexts.ImageUploadFailed_Subject, ErrorsTexts.ImageUploadFailed_FileNotFound);
                this.LogError("AppletResult.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            this.Logger.Error(string.Format("FileEnd. Error result for weborbId: {0}", id));
        }

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string InitializeFileUpload(FileDTO file)
        {
            ValidationResult validationResult;
            Error error;
            if (this.IsValid(file, out validationResult))
            {
                User user = file.createdBy.HasValue ? this.UserModel.GetOneById(file.createdBy.Value).Value : null;
                if (user != null)
                {
                    File createdFile = this.FileModel.CreateFile(user, file.fileName, DateTime.Now, file.width, file.height, file.x, file.y);
                    return createdFile.Id.ToString();
                }

                error = new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER, 
                        ErrorsTexts.ImageUploadInitialization_Subject, 
                        ErrorsTexts.ImageUploadInitialization_CurrentUserNotFound);
            }
            else
            {
                error = this.GenerateValidationError(validationResult);
            }

            if (error != null)
            {
                this.Logger.Error(
                    string.Format(
                        "ImageUploadInitialization. Error result for user: {0}, Error code: {1}, Error Message: {2}",
                        file.createdBy ?? 0,
                        error.errorCode,
                        error.errorMessage));
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return null;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="FileDTO"/>.
        /// </returns>
        public FileDTO Save(FileDTO dto)
        {
            ValidationResult result;
            if (this.IsValid(dto, out result))
            {
                FileModel companyModel = this.FileModel;
                File instance = companyModel.GetOneById(dto.fileId).Value;
                if (instance != null)
                {
                    instance = this.ConvertDto(dto, instance);
                    companyModel.RegisterSave(instance, true);
                    return new FileDTO(instance);
                }

                var error = new Error(Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, ErrorsTexts.GetResultError_NotFound, ErrorsTexts.EntityCreationError_Subject);
                this.LogError("File.Save", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            var validationError = this.GenerateValidationError(result);
            this.LogError("File.Save", validationError);
            throw new FaultException<Error>(validationError, validationError.errorMessage);
        }

        ///// <summary>
        ///// The get by id.
        ///// </summary>
        ///// <param name="id">
        ///// The id.
        ///// </param>
        ///// <returns>
        ///// The <see cref="FileDTO"/>.
        ///// </returns>
        //public FileDTO GetById(Guid id)
        //{
        //    File session;
        //    if ((session = this.FileModel.GetOneById(id).Value) == null)
        //    {
        //        var error = new Error(Errors.CODE_ERRORTYPE_OBJECT_NOT_FOUND, ErrorsTexts.EntityGetError_Subject, ErrorsTexts.GetResultError_NotFound);
        //        this.LogError("File.GetById", error);
        //        throw new FaultException<Error>(error, error.errorMessage);
        //    }

        //    return new FileDTO(session);
        //}

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

        public FileDTO GetById(Guid id)
        {
            var file = FileModel.GetOneById(id).Value;
            var result = new FileDTO(file);
            return result;
        }

        #endregion
    }
}