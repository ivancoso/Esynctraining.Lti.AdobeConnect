namespace EdugameCloud.WCFService.Contracts
{
    using System;
    using System.ServiceModel;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Lti.DTO;
    using EdugameCloud.WCFService.DTO;

    using Esynctraining.Core.Domain.Entities;

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
        [OperationContract]
        [FaultContract(typeof(Error))]
        void ActivateById(int companyId);

        /// <summary>
        /// The deactivate by id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void DeactivateById(int companyId);

        /// <summary>
        /// Deletes company by id.
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
        /// The request license upgrade.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(Error))]
        void RequestLicenseUpgrade(int companyId);

        /// <summary>
        ///     The all.
        /// </summary>
        /// <returns>
        ///     The <see cref="CompanyDTO" />.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyDTO[] GetAll();

        /// <summary>
        /// The get license history by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLicenseDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLicenseDTO[] GetLicenseHistoryByCompanyId(int companyId);

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyDTO GetById(int id);

        /// <summary>
        /// Gets theme by company id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyThemeDTO GetThemeByCompanyId(int id);

        /// <summary>
        /// The get theme by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyThemeDTO GetThemeById(string id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyDTO Save(CompanyDTO resultDto);

        /// <summary>
        /// The save theme.
        /// </summary>
        /// <param name="companyThemeDTO">
        /// The company theme DTO.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyThemeDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyThemeDTO SaveTheme(CompanyThemeDTO companyThemeDTO);

        /// <summary>
        /// The get LMS history by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="CompanyLmsDTO"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(Error))]
        CompanyLmsDTO[] GetLMSHistoryByCompanyId(int companyId);

        #endregion
    }
}