namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN session member interface.
    /// </summary>
    [ServiceContract]
    public interface ISNMemberService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets all members.
        /// </summary>
        /// <param name="sessionId">
        /// The AC Session Id.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNMemberDTO[] GetAllByACSessionId(int sessionId);

        /// <summary>
        /// Deletes member by id.
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
        /// Get member by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNMemberDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="sessionMember">
        /// The member.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNMemberDTO Save(SNMemberDTO sessionMember);

        /// <summary>
        /// Saves all members.
        /// </summary>
        /// <param name="sessionMember">
        /// The member.
        /// </param>
        /// <returns>
        /// The <see cref="SNMemberDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNMemberSaveAllDTO SaveAll(SNMemberDTO[] sessionMember);

        #endregion
    }
}