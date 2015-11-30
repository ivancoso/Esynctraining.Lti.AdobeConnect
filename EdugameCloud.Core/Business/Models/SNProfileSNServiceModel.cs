namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    ///     The SN Profile SN Service model.
    /// </summary>
    public class SNProfileSNServiceModel : BaseModel<SNProfileSNService, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileSNServiceModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNProfileSNServiceModel(IRepository<SNProfileSNService, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}