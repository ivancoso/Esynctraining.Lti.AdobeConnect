namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The Email Service interface.
    /// </summary>
    [ServiceContract]
    public interface IEmailService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<EmailHistoryDTO> GetHistory();

        /// <summary>
        /// Saves the history item.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<EmailHistoryDTO> SaveHistory(EmailHistoryDTO dto);

        /// <summary>
        /// Gets history by company id.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<EmailHistoryDTO> GetHistoryByCompanyId(int companyId);

        /// <summary>
        /// The resend.
        /// </summary>
        /// <param name="emailHistoryId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<EmailHistoryDTO> ResendEmail(int emailHistoryId);

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<NewsletterSubscriptionDTO> SubscribeToNewsletter(string email);


        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>      
        [OperationContract]
        ServiceResponse<NewsletterSubscriptionDTO> GetNewsletterSubscription();

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<UserDTO> SetUserUnsubscribed(string email);

        #endregion
    }
}
