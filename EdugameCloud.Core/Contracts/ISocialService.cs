namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The ProxyService interface.
    /// </summary>
    [ServiceContract]
    public interface ISocialService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The proxy for sending data over web.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<string> Proxy(WebRequestDTO dto);

        #endregion
    }
}
