namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;

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
        [FaultContract(typeof(Error))]
        LmsUserParametersDTO Save(LmsUserParametersDTO param);
    }
}
