namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SubModule model.
    /// </summary>
    public class SubModuleModel : BaseModel<SubModule, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SubModuleModel(IRepository<SubModule, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}