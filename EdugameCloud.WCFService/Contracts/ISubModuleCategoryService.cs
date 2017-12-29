namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The SubModuleCategory Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISubModuleCategoryService
    {
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "DeleteById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int DeleteById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        SubModuleCategoryDTO[] GetByUser(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAppletCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetAppletCategoriesByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO GetById(int id);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetQuizCategoriesByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSNProfileCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetSNProfileCategoriesByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveyCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetSurveyCategoriesByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTestCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetTestCategoriesByUserId(int userId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO Save(SubModuleCategoryDTO resultDto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "SaveAll", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategorySaveAllDTO SaveAll(SubModuleCategoryDTO[] results);

    }

}