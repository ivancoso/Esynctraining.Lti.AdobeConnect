namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNGroupDiscussionDTO GetById(int id);

        /// <summary>
        /// Get by AC session id.
        /// </summary>
        /// <param name="sessionId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNGroupDiscussionDTO GetByAcSessionId(int sessionId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="SNGroupDiscussionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNGroupDiscussionDTO Save(SNGroupDiscussionDTO build);

        #endregion
    }
}