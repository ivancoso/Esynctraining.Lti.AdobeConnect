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

    }

}