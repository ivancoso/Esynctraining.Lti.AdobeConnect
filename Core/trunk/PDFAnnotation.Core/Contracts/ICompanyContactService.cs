namespace PDFAnnotation.Core.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The Company interface.
    /// </summary>
    [ServiceContract]
    public interface ICompanyContactService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="searchPattern">
        /// The search Pattern.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyContactExDTO> GetContactsPagedByCompany(string searchPattern, int companyId, int pageIndex, int pageSize);

        /// <summary>
        /// Deletes user by id.
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
        /// Get user by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyContactExDTO> GetById(int id);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="companyContactDTO">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyContactExDTO> Save(CompanyContactExDTO companyContactDTO);

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="contactIds">
        /// The contact ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyContactExDTO> GetAllByIds(List<int> contactIds);

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <param name="contactIds">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CompanyContactExDTO> GetAllByCompanyAndIds(int companyId, List<int> contactIds);

        #endregion
    }
}