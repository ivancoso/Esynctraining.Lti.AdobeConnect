namespace EdugameCloud.WCFService.Contracts
{
    using System;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.DTO;

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
        /// Deletes company theme by id.
        /// </summary>
        /// <param name="id">
        /// The id
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse{Guid}"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<Guid> DeleteThemeById(Guid id);

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
        /// The get license history by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyLicenseDTO> GetLicenseHistoryByCompanyId(int companyId);

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
        /// Gets theme by company id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyThemeDTO> GetThemeByCompanyId(int id);

        /// <summary>
        /// The get theme by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyThemeDTO> GetThemeById(Guid id);

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

        /// <summary>
        /// The save theme.
        /// </summary>
        /// <param name="companyThemeDTO">
        /// The company theme DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyThemeDTO> SaveTheme(CompanyThemeDTO companyThemeDTO);

        /// <summary>
        /// The get LMS history by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyLmsDTO> GetLMSHistoryByCompanyId(int companyId);

        #endregion
    }
}