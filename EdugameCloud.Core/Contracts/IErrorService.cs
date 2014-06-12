namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

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
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse LogError(string message, string details);

        /// <summary>
        /// Sends mail about error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse SendEmailAboutError(string message, string details);

        #endregion
    }
}