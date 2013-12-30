namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The ScoreType model.
    /// </summary>
    public class ScoreTypeModel : BaseModel<ScoreType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ScoreTypeModel(IRepository<ScoreType, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}