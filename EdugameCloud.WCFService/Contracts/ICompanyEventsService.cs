using System.ServiceModel;
using System.ServiceModel.Web;
using EdugameCloud.Core.Domain.DTO;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.WCFService.Contracts
{
    [ServiceContract]
    public interface ICompanyEventsService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyQuizEventMappingDTO GetById(int eventQuizMappingId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetEventsByCompany?companyId={companyId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyEventDTO[] GetEventsByCompany(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetEventsByCompanyAcServer?companyAcServerId={companyAcServerId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyEventDTO[] GetEventsByCompanyAcServer(int companyAcServerId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetEventQuizMappings", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyQuizEventMappingDTO[] GetEventQuizMappings();

        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetEventQuizMappingsByCompanyId?companyId={companyId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyQuizEventMappingDTO[] GetEventQuizMappingsByCompanyId(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        //[WebGet(UriTemplate = "GetEventQuizMappingsByAcServerId?acServerId={acServerId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        CompanyQuizEventMappingDTO[] GetEventQuizMappingsByAcServerId(int acServerId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyQuizEventMappingSaveDTO Save(CompanyQuizEventMappingSaveDTO eventQuizMapping);
    }
}
