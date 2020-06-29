namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ICompanyLicenseService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO Save(CompanyLicenseDTO dto);

    }

}