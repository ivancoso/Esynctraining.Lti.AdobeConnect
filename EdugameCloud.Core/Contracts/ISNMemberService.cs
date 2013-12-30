namespace EdugameCloud.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Domain.Contracts;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNMemberDTO> GetAllByACSessionId(int sessionId);

        /// <summary>
        /// Deletes member by id.
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
        /// Get member by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNMemberDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="sessionMember">
        /// The member.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNMemberDTO> Save(SNMemberDTO sessionMember);

        /// <summary>
        /// Saves all members.
        /// </summary>
        /// <param name="sessionMember">
        /// The member.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SNMemberDTO> SaveAll(List<SNMemberDTO> sessionMember);

        #endregion
    }
}