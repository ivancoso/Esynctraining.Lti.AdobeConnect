namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    /// The BuildVersion model.
    /// </summary>
    public class BuildVersionModel : BaseModel<BuildVersion, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildVersionModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public BuildVersionModel(IRepository<BuildVersion, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}