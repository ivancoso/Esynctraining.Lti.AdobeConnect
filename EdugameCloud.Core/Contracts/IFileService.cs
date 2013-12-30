namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

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
        /// Should remove uploaded file file if any and instance from db
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
        /// The save.
        /// </summary>
        /// <param name="file">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> Save(FileDTO file);

        /// <summary>
        /// Get file by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<FileDTO> GetById(int id);



        #endregion
    }
}