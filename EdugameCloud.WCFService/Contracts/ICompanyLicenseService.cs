namespace EdugameCloud.WCFService.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Entities;

    [ServiceContract]
    public interface ICompanyLicenseService
    {
        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int DeleteById(int id);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="dto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO Save(CompanyLicenseDTO dto);

        /// <summary>
        /// The update seats count.
        /// </summary>
        /// <param name="licenseId">
        /// The license id.
        /// </param>
        /// <param name="seatsCount">
        /// The seats count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        int UpdateSeatsCount(int licenseId, int seatsCount);

    }

}