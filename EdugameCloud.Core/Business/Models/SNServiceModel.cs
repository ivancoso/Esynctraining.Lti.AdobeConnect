namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SN Service model.
    /// </summary>
    public class SNServiceModel : BaseModel<SNService, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNServiceModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNServiceModel(IRepository<SNService, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}