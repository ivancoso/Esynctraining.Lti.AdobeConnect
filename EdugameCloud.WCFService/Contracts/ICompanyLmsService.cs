namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Company LMS Service.
    /// </summary>
    [ServiceContract]
    public interface ICompanyLmsService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLmsDTO Save(CompanyLmsDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        ConnectionInfoDTO TestConnection(ConnectionTestDTO resultDto);

    }

}
