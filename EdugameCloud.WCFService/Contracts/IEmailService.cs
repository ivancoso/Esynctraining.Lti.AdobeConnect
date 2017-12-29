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
        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO[] GetHistory();

        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO SaveHistory(EmailHistoryDTO dto);

        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO[] GetHistoryByCompanyId(int companyId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        EmailHistoryDTO ResendEmail(int emailHistoryId);

        [OperationContract]
        [FaultContract(typeof(Error))]
        NewsletterSubscriptionDTO SubscribeToNewsletter(string email);

        [OperationContract]
        [FaultContract(typeof(Error))]
        NewsletterSubscriptionDTO[] GetNewsletterSubscription();

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
