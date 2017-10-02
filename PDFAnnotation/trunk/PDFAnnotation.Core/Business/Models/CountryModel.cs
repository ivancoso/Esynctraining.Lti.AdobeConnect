using Esynctraining.NHibernate;

namespace PDFAnnotation.Core.Business.Models
{
    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The country model.
    /// </summary>
    public class CountryModel : BaseModel<Country, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CountryModel(IRepository<Country, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}