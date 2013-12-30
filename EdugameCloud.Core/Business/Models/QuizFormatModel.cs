namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The QuizFormat model.
    /// </summary>
    public class QuizFormatModel : BaseModel<QuizFormat, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizFormatModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuizFormatModel(IRepository<QuizFormat, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}