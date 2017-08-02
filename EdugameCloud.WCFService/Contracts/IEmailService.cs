namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Email Service interface.
    /// </summary>
    [ServiceContract]
    public interface IEmailService
    {
        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO[] GetHistory();

        /// <summary>
        /// Saves the history item.
        /// </summary>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO SaveHistory(EmailHistoryDTO dto);

        /// <summary>
        /// Gets history by company id.
        /// </summary>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO[] GetHistoryByCompanyId(int companyId);

        /// <summary>
        /// The resend.
        /// </summary>
        /// <param name="emailHistoryId">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO ResendEmail(int emailHistoryId);

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="NewsletterSubscriptionDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        NewsletterSubscriptionDTO SubscribeToNewsletter(string email);


        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <returns>
        /// The <see cref="NewsletterSubscriptionDTO"/>.
        /// </returns>      
        [OperationContract]
        [FaultContract(typeof(Error))]
        NewsletterSubscriptionDTO[] GetNewsletterSubscription();

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        UserDTO[] SetUserUnsubscribed(string email);

        /// <summary>
        /// Send live quiz result email for goddard
        /// </summary>
        /// <param name="quizResultIds"></param>
        /// <returns>OperationResultDto</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        OperationResultDto SendEventQuizResultEmail(int[] quizResultIds);

        /// <summary>
        /// Send email about registration to a user for goddard
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <returns>OperationResultDto</returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        OperationResultDto SendRegistrationEmail(EventRegistrationDTO registrationInfo);

    }

}
