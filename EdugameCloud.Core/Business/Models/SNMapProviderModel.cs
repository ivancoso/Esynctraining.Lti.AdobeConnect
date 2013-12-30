namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SN Map provider model.
    /// </summary>
    public class SNMapProviderModel : BaseModel<SNMapProvider, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapProviderModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNMapProviderModel(IRepository<SNMapProvider, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}