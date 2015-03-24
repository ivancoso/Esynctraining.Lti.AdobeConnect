//namespace EdugameCloud.WCFService.Contracts
//{
//    using System.ServiceModel;
//    using EdugameCloud.Core.Domain.DTO;

//    using Esynctraining.Core.Domain.Entities;

//    /// <summary>
//    ///     The ProxyService interface.
//    /// </summary>
//    [ServiceContract]
//    public interface ISocialService
//    {
//        #region Public Methods and Operators

//        /// <summary>
//        /// The all.
//        /// </summary>
//        /// <param name="key">
//        /// The key.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SocialUserTokensDTO"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        SocialUserTokensDTO GetSocialUserTokens(string key);

//        /// <summary>
//        /// The list subscriptions.
//        /// </summary>
//        /// <returns>
//        /// The <see cref="SubscriptionDTO"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        SubscriptionDTO ListSubscriptions();

//        /// <summary>
//        /// The proxy for sending data over web.
//        /// </summary>
//        /// <param name="dto">
//        /// The DTO.
//        /// </param>
//        /// <returns>
//        /// The <see cref="string"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        string Proxy(WebRequestDTO dto);

//        /// <summary>
//        /// The subscribe to tag.
//        /// </summary>
//        /// <param name="tag">
//        /// The tag.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        SubscriptionResDTO SubscribeToTag(string tag);

//        /// <summary>
//        /// The subscribe to tag.
//        /// </summary>
//        /// <param name="tags">
//        /// The tags.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionUpdateSumDTO"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        SubscriptionUpdateSumDTO[] GetUpdatesForTags(TagRequestDTO[] tags);

//        /// <summary>
//        /// The unsubscribe to tag.
//        /// </summary>
//        /// <param name="tag">
//        /// The tag.
//        /// </param>
//        /// <returns>
//        /// The <see cref="SubscriptionResDTO"/>.
//        /// </returns>
//        [OperationContract]
//        [FaultContract(typeof(Error))]
//        SubscriptionResDTO UnsubscribeTag(string tag);

//        #endregion
//    }
//}