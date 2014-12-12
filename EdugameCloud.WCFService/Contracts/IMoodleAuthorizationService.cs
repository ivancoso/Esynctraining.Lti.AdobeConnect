namespace EdugameCloud.Lti.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The MOODLE authorization service interface
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
