namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The Company LMS Service interface.
    /// </summary>
    [ServiceContract]
    public interface ICompanyLmsService
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLmsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLmsDTO Save(CompanyLmsDTO resultDto);

        /// <summary>
        /// The test connection.
        /// </summary>
        /// <param name="resultDto">
        /// The result DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ConnectionInfoDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        ConnectionInfoDTO TestConnection(ConnectionTestDTO resultDto);
    }
}
