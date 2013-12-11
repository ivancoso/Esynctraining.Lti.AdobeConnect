namespace PDFAnnotation.Core.Business.Models
{
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The address model.
    /// </summary>
    public class AddressModel : BaseModel<Address, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public AddressModel(IRepository<Address, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}