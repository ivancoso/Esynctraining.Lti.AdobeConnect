namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using Esynctraining.Core.Logging;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;
    using Esynctraining.Http.Streaming;

    using FluentValidation;
    using FluentValidation.Results;

    /// <summary>
    /// The document uploader.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall,
        IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Uploader : IUploader
    {
        /// <summary>
        /// The file model.
        /// </summary>
        private FileModel FileModel
        {
            get
            {
                return IoC.Resolve<FileModel>();
            }
        }

        /// <summary>
        /// Gets the current request.
        /// </summary>
        public HttpRequestMessageProperty CurrentRequest
        {
            get
            {
                object authValue;
                OperationContext.Current.IncomingMessageProperties.TryGetValue("httpRequest", out authValue);
                if (authValue != null)
                {
                    return (HttpRequestMessageProperty)authValue;
                }

                return null;
            }
        }

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public FilesUploadedResultDTO uploadMultipart(string fileId, Stream stream)
        {
            try
            {
                var request = this.CurrentRequest;
                var requestHeaders = request.Headers;

                Guid fileIdVal;
                if (string.IsNullOrWhiteSpace(fileId) || !Guid.TryParse(fileId, out fileIdVal))
                {
                    throw new ApplicationException("fileId is not passed or not a GUID");
                }

                string contentType = requestHeaders["Content-Type"];
                int boundaryIndex = contentType.With(x => x.IndexOf("boundary=", StringComparison.Ordinal));
                var streamedFiles = new HttpMultipart(
                    stream,
                    boundaryIndex > 0 ? contentType.Substring(boundaryIndex + 9).Trim() : null);
                var createdFiles = new List<Guid>();
                var notSavedFiles = new Dictionary<string, string>();
                foreach (var httpMultipartBoundary in streamedFiles.GetBoundaries())
                {
                    var fileName = string.IsNullOrWhiteSpace(httpMultipartBoundary.Filename)
                                       ? httpMultipartBoundary.Name
                                       : httpMultipartBoundary.Filename;
                    if (fileName.Equals("FileName", StringComparison.InvariantCultureIgnoreCase)
                        || fileName.Equals("Upload", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var file = new UploadedFileDTO
                    {
                        fileId = fileIdVal,
                        fileName = fileName,
                        contentType = httpMultipartBoundary.ContentType,
                        content = httpMultipartBoundary.Value.ReadFully(),
                        dateCreated = DateTime.Now,
                    };

                    this.FileModel.SaveWeborbFile(file);
                    createdFiles.Add(fileIdVal);
                }

                return this.FormatSuccessfulResponse(createdFiles, notSavedFiles);
            }
            catch (Exception ex)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "File upload unhandled exception", ex.ToString());
                IoC.Resolve<ILogger>().Error("File upload unhandled exception", ex);
                throw new FaultException<Error>(error, error.errorMessage);
            }
        }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="obj">
        /// The object to check.
        /// </param>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to check
        /// </typeparam>
        /// <returns>
        /// The validation result <see cref="bool"/>.
        /// </returns>
        public bool IsValid<T>(T obj, out ValidationResult validationResult)
        {
            validationResult = null;
            try
            {
                validationResult = IoC.Resolve<IValidator<T>>().Validate(obj);
                return validationResult.IsValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// The format successful response.
        /// </summary>
        /// <param name="savedfiles">
        /// The saved files.
        /// </param>
        /// <param name="notSavedfiles">
        /// The not saved files.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private FilesUploadedResultDTO FormatSuccessfulResponse(List<Guid> savedfiles, Dictionary<string, string> notSavedfiles)
        {
            var result = new FilesUploadedResultDTO
                             {
                                 savedIds = savedfiles.ToArray(),
                                 failedFiles = notSavedfiles.Select(x => new FailedFileDTO { error = x.Value, fileName = x.Key }).ToArray()
                             };
            return result;
        }
    }
}
