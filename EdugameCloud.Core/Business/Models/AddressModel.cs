namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

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