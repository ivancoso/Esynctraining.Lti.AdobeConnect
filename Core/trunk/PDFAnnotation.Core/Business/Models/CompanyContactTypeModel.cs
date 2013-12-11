namespace PDFAnnotation.Core.Business.Models
{
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContactType model class.
    /// </summary>
    public class CompanyContactTypeModel : BaseModel<CompanyContactType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactTypeModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyContactTypeModel(IRepository<CompanyContactType, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}