namespace PDFAnnotation.Core.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    ///     The Category interface.
    /// </summary>
    [ServiceContract]
    public interface ICategoryService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CategoryDTO> GetAllByCompanyId(int companyId);

        /// <summary>
        /// The get all for contact id.
        /// </summary>
        /// <param name="contactId">
        /// The contact id.
        /// </param>
        /// <param name="includeDetails">
        /// The include details.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CategoryDTO> GetAllForContactId(int contactId, bool includeDetails);

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="searchPattern">
        /// The filter By Name.
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
        ServiceResponse<CategoryDTO> GetCompanyCategoriesPaged(string searchPattern, int companyId, int pageIndex, int pageSize);

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchPattern">
        /// The search pattern.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="maxRows">
        /// The max rows.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CategoryDTO> Search(string searchPattern, int companyId, int maxRows);

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
        /// Get category by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="includeDetails">
        /// The include Details.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CategoryDTO> GetById(int id, bool includeDetails);

        /// <summary>
        /// Get category by id.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse<CategoryDTO> Save(CategoryDTO category);

        #endregion
    }
}