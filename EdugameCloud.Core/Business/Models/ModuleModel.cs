namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The Module model.
    /// </summary>
    public class ModuleModel : BaseModel<Module, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ModuleModel(IRepository<Module, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}