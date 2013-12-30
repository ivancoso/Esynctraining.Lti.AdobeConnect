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
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse LogError(string message, string details);

        #endregion
    }
}