namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.DTO;
    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ICompanyService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        void ActivateById(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        void DeactivateById(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        void RequestLicenseUpgrade(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyFlatDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyFlatDTO[] GetByLmsCompanyConsumerKey(string consumerKey);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO[] GetLicenseHistoryByCompanyId(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyThemeDTO GetThemeByCompanyId(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetThemeById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyThemeDTO GetThemeById(string id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyDTO Save(CompanyDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyThemeDTO SaveTheme(CompanyThemeDTO companyThemeDTO);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLmsDTO[] GetLMSHistoryByCompanyId(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyFlatDTO[] GetByAdvancedFilter(CompanyAdvancedFilterDTO filter);

    }

}