namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Build Version interface.
    /// </summary>
    [ServiceContract]
    public interface ISNGroupDiscussionService 
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes build by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        /// <summary>
        /// Get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNGroupDiscussionDTO> GetById(int id);

        /// <summary>
        /// Get by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNGroupDiscussionDTO> GetByAcSessionId(int sessionId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNGroupDiscussionDTO> Save(SNGroupDiscussionDTO build);

        #endregion
    }
}