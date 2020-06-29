namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface IACSessionService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        ACSessionDTO Save(ACSessionDTO session);

    }

}