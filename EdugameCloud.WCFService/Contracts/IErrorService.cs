namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IErrorService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void LogError(string message, string details);

        /// <summary>
        /// Sends mail about error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void SendEmailAboutError(string message, string details);

        #endregion
    }
}