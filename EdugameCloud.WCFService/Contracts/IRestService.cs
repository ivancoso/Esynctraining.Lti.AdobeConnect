namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The RestService interface.
    /// </summary>
    [ServiceContract]
    public interface IRestService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The activate.
        /// </summary>
        /// <param name="activationCode">
        /// The activation code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "Activate/{activationCode}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        string Activate(string activationCode);

        /// <summary>
        /// The activate.
        /// </summary>
        /// <returns>
        /// The <see cref="CountryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "Countries", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        CountryDTO[] GetCountries();

        /// <summary>
        /// The activate.
        /// </summary>
        /// <returns>
        /// The <see cref="StateDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebGet(UriTemplate = "States", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        StateDTO[] GetStates();

        #endregion
    }
}