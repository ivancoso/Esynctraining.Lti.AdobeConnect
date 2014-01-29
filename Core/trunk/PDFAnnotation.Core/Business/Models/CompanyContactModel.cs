namespace PDFAnnotation.Core.Business.Models
{
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The CompanyContact model.
    /// </summary>
    public class CompanyContactModel : BaseModel<CompanyContact, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyContactModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyContactModel(IRepository<CompanyContact, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}