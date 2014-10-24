namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The moodle authorization service interface
    /// </summary>
    [ServiceContract]
    public interface IMoodleAuthorizationService
    {
        /// <summary>
        /// The save
        /// </summary>
        /// <param name="param">
        /// The user parameters
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract(Action = "Save")]
        ServiceResponse<LmsUserParametersDTO> Save(LmsUserParametersDTO param);

    }
}
