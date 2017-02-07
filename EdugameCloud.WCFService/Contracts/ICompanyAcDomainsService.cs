using System.ServiceModel;
using System.ServiceModel.Web;
using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.WCFService.Contracts
{
    [ServiceContract]
    public interface ICompanyAcDomainsService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetAllByCompany", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [WebGet(UriTemplate = "GetAllByCompany?companyId={companyId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyAcDomainDTO[] GetAllByCompany(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyAcDomainDTO Save(CompanyAcDomainDTO acDomain);
    }
}