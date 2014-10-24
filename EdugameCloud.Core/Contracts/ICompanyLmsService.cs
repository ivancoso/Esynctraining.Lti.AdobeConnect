namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    /// The CompanyLmsService interface.
    /// </summary>
    [ServiceContract]
    public interface ICompanyLmsService
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The result dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyLmsDTO> Save(CompanyLmsDTO resultDto);

        /// <summary>
        /// The test connection.
        /// </summary>
        /// <param name="resultDto">
        /// The result dto.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<ConnectionInfoDTO> TestConnection(CompanyLmsDTO resultDto);
    }
}
