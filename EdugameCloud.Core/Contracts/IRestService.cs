namespace EdugameCloud.Core.Contracts
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

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
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Activate/{activationCode}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        ServiceResponse<string> Activate(string activationCode);

        /// <summary>
        /// The activate.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "Countries", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse<CountryDTO> GetCountries();

        /// <summary>
        /// The activate.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "States", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse<StateDTO> GetStates();

        #endregion
    }
}