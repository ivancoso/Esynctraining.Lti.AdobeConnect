namespace EdugameCloud.WCFService.Contracts
{
    using System;
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        /// The <see cref="FileDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO FileUploadEnd(string webOrbId);

        /// <summary>
        /// Should remove uploaded file file if any and instance from DB
        /// </summary>
        /// <param name="webOrbId">
        /// The file id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void FileUploadFailed(string webOrbId);

        /// <summary>
        /// The file start.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        string InitializeFileUpload(FileDTO file);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="file">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="FileDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO Save(FileDTO file);

        /// <summary>
        /// Get file by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="FileDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        FileDTO GetById(Guid id);

        #endregion
    }
}