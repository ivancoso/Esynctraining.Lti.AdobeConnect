namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.DTO;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Company LMS Service.
    /// </summary>
    [ServiceContract]
    public interface ICompanyLmsService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLmsOperationDTO Save(CompanyLmsDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        ConnectionInfoDTO TestConnection(ConnectionTestDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

    }

}
