namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The BuildVersionType model.
    /// </summary>
    public class BuildVersionTypeModel : BaseModel<BuildVersionType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public BuildVersionTypeModel(IRepository<BuildVersionType, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}