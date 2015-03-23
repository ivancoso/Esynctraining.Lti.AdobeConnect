namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SubModuleCategory Service interface.
    /// </summary>
    [ServiceContract]
    public interface ISubModuleCategoryService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "DeleteById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int DeleteById(int id);

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="SubModuleCategoryDTO" />.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAll", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetAll();

        [OperationContract]
        [FaultContract(typeof(Error))]
        SubModuleCategoryDTO[] GetByUser(int userId);

        /// <summary>
        /// The get applet categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetAppletCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetAppletCategoriesByUserId(int userId);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetById?id={id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO GetById(int id);

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="PagedSubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetPaged?pageIndex={pageIndex}&pageSize={pageSize}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        PagedSubModuleCategoryDTO GetPaged(int pageIndex, int pageSize);

        /// <summary>
        /// The get quiz categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetQuizCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetQuizCategoriesByUserId(int userId);

        /// <summary>
        /// The get SN profile categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSNProfileCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetSNProfileCategoriesByUserId(int userId);

        /// <summary>
        /// The get survey categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetSurveyCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetSurveyCategoriesByUserId(int userId);

        /// <summary>
        /// The get test categories by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "GetTestCategoriesByUserId?userId={userId}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO[] GetTestCategoriesByUserId(int userId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategoryDTO Save(SubModuleCategoryDTO resultDto);

        /// <summary>
        /// The save questions results.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "SaveAll", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        SubModuleCategorySaveAllDTO SaveAll(SubModuleCategoryDTO[] results);

        #endregion
    }
}