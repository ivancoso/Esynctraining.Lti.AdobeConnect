namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    /// The QuizQuestionResult model.
    /// </summary>
    public class QuizQuestionResultModel : BaseModel<QuizQuestionResult, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionResultModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public QuizQuestionResultModel(IRepository<QuizQuestionResult, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}