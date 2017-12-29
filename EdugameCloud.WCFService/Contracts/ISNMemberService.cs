namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ISNMemberService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        SNMemberSaveAllDTO SaveAll(SNMemberDTO[] sessionMember);

    }

}