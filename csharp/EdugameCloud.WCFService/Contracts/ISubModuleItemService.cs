namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The SubModuleItem Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISubModuleItemService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "DeleteById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAppletSubModuleItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetAppletSubModuleItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetPaged?pageIndex={pageIndex}&pageSize={pageSize}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        // IS: Checked. is in use.
        PagedSubModuleItemsDTO GetPaged(int pageIndex, int pageSize);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizSubModuleItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetQuizSubModuleItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSNProfileSubModuleItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetSNProfileSubModuleItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveySubModuleItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetSurveySubModuleItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTestSubModuleItemsByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO[] GetTestSubModuleItemsByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemDTO Save(SubModuleItemDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "SaveAll", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleItemSaveAllDTO SaveAll(SubModuleItemDTO[] results);

    }

}