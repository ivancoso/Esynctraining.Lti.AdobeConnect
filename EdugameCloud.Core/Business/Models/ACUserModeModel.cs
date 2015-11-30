namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    ///     The ACUserMode model.
    /// </summary>
    public class ACUserModeModel : BaseModel<ACUserMode, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACUserModeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ACUserModeModel(IRepository<ACUserMode, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}