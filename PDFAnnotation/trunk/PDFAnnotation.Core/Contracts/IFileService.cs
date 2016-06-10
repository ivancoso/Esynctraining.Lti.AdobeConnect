using Esynctraining.Core.Domain.Entities;

namespace PDFAnnotation.Core.Contracts
{
    using System;
    using System.Collections.Generic;
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
        /// <param name="Id">
        /// The file id.
        /// </param>
        /// <param name="fileStatus">
        /// The file Status.
        /// </param>
        /// <returns>
        /// The FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO FileUploadEnd(string Id, int fileStatus);

        /// <summary>
        /// Should remove uploaded image file if any and instance from DB
        /// </summary>
        /// <param name="Id">
        /// The file id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void FileUploadFailed(string Id);

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The String.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string InitializeFileUpload(FileDTO file);

        /// <summary>
        /// The get files by case and dates.
        /// </summary>
        /// <param name="searchPattern">
        /// The searchPattern.
        /// </param>
        /// <param name="categoryId">
        /// The case id.
        /// </param>
        /// <param name="start">
        /// The start Date.
        /// </param>
        /// <param name="end">
        /// The end Date.
        /// </param>
        /// <returns>
        /// The array of FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO[] GetFilesByDates(string searchPattern, int categoryId, double start, double end);

        /// <summary>
        /// Gets event's files.
        /// </summary>
        /// <param name="categoryId">
        /// The case Id.
        /// </param>
        /// <returns>
        /// The array of FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO[] GetFilesByCategory(int categoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns>The FileConvertationDTO</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileConvertationDTO WasFileConvertedToSwf(string id, int? page = null);

        /// <summary>
        /// The get exhibits report.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The array of FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO[] GetFilesReport(int companyId, double? start, double? end);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="ids">
        /// The id array.
        /// </param>
        /// <returns>
        /// The id
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        Guid DeleteByIds(string[] ids);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO Save(FileDTO dto);

        /// <summary>
        /// The get preloaded exhibits.
        /// </summary>
        /// <param name="contactId">
        /// The contact id.
        /// </param>
        /// <param name="acMeetingUrl">
        /// The ac Meeting Url.
        /// </param>
        /// <param name="includeSharedToFirm">
        /// The include Shared To Firm.
        /// </param>
        /// <returns>
        /// The array of FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO[] GetPreloadedFiles(int contactId, string acMeetingUrl, bool includeSharedToFirm = true);

        /// <summary>
        /// Checks supposed exhibit number, returns this or first available
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The FileDTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO CheckFileNumber(FileDTO dto);

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
        /// The exhibit number
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int CheckCaseFileNumber(int categoryId, int topicId, int? fileNumber);

        /// <summary>
        /// Creates a new empty PDF file
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// file DTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO Create(FileDTO dto);

        /// <summary>
        /// The update status.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// file DTO
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO UpdateStatus(string fileId, int status);


        #endregion
    }
}