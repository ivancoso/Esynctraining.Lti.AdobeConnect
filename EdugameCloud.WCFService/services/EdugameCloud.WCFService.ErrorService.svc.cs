namespace EdugameCloud.WCFService
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Contracts;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ErrorService : BaseService, IErrorService
    {
        #region Properties

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Logs error to server log.
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
        public ServiceResponse LogError(string message, string details)
        {
            this.logger.Error(
                    string.Format(
                        "LogError from Client: Error Message: {0}, Error Details: {1}, Client: {2}",
                        message,
                        details,
                        CurrentUser.With(x => x.Email)));
            return new ServiceResponse { status = Errors.CODE_RESULTTYPE_SUCCESS };
        }

        #endregion
    }
}