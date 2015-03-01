namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IACSessionService
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
        int DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ACSessionDTO GetById(int id);

        /// <summary>
        /// The get by SMI id.
        /// </summary>
        /// <param name="smiId">
        /// The SMI id.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ACSessionDTO[] GetBySMIId(int smiId);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ACSessionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        [WebInvoke(UriTemplate = "Save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        ACSessionDTO Save(ACSessionDTO user);

        #endregion
    }
}