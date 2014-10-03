namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    [ServiceContract]
    public interface ICompanyLmsService
    {
        [OperationContract]
        ServiceResponse<CompanyLmsDTO> Save(CompanyLmsDTO resultDto);

        [OperationContract]
        ServiceResponse<ConnectionInfoDTO> TestConnection(CompanyLmsDTO resultDto);
    }
}
