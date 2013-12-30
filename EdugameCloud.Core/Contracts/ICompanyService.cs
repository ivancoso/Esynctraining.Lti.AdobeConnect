// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompanyService.cs" company="">
//   
// </copyright>
// <summary>
//   The Company Service interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The Company Service interface.
    /// </summary>
    [ServiceContract]
    public interface ICompanyService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The activate by id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse ActivateById(int companyId);

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse DeactivateById(int companyId);

        /// <summary>
        /// Deletes company by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<int> DeleteById(int id);

        /// <summary>
        /// The request license upgrade.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse RequestLicenseUpgrade(int companyId);

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyDTO> GetAll();

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyDTO> Save(CompanyDTO resultDto);

        #endregion
    }
}