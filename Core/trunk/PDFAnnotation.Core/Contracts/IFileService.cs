namespace PDFAnnotation.Core.Contracts
{
    using System;
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IFileService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The file end.
        /// </summary>
        /// <param name="webOrbId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> FileUploadEnd(string webOrbId);

        /// <summary>
        /// Should remove uploaded image file if any and instance from DB
        /// </summary>
        /// <param name="webOrbId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse FileUploadFailed(string webOrbId);

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{String}"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<string> InitializeFileUpload(FileDTO file);

        /// <summary>
        /// The get files by case and dates.
        /// </summary>
        /// <param name="searchPattern">
        /// The searchPattern.
        /// </param>
        /// <param name="caseId">
        /// The case id.
        /// </param>
        /// <param name="start">
        /// The start Date.
        /// </param>
        /// <param name="end">
        /// The end Date.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> GetFilesByDates(string searchPattern, int caseId, DateTime start, DateTime end);

        /// <summary>
        /// Gets event's files.
        /// </summary>
        /// <param name="caseId">
        /// The case Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> GetFilesByCase(int caseId);

        /// <summary>
        /// The get exhibits report.
        /// </summary>
        /// <param name="caseId">
        /// The case id.
        /// </param>
        /// <param name="firmId">
        /// The company id.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> GetFilesReport(int companyId, DateTime? start, DateTime? end);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> Save(FileDTO dto);

        /// <summary>
        /// Checks supposed exhibit number, returns this or first available
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> CheckFileNumber(FileDTO dto);

        /// <summary>
        /// Checks supposed exhibit number, returns this or first available
        /// </summary>
        /// <param name="categoryId">
        /// The case Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="fileNumber">
        /// The file Number.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> CheckCaseFileNumber(int categoryId, int topicId, int? fileNumber);

        /// <summary>
        /// Creates a new empty PDF file
        /// </summary>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="originFileId">
        /// The origin File Id.
        /// </param>
        /// <returns>
        /// file dto
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> Create(int categoryId, int topicId, int originFileId);

        #endregion
    }
}