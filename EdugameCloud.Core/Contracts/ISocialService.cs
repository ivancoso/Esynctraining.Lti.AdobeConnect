namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The ProxyService interface.
    /// </summary>
    [ServiceContract]
    public interface ISocialService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SocialUserTokensDTO> GetSocialUserTokens(string key);

        /// <summary>
        /// The list subscriptions.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubscriptionDTO> ListSubscriptions();

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

        /// <summary>
        /// The subscribe to tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubscriptionResDTO> SubscribeToTag(string tag);

        /// <summary>
        /// The subscribe to tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubscriptionUpdateDTO> GetUpdatesForTag(string tag);

        /// <summary>
        /// The unsubscribe to tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<SubscriptionResDTO> UnsubscribeTag(string tag);

        #endregion
    }
}