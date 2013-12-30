namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SurveyQuestionResultAnswer model.
    /// </summary>
    public class SurveyQuestionResultAnswerModel : BaseModel<SurveyQuestionResultAnswer, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultAnswerModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SurveyQuestionResultAnswerModel(IRepository<SurveyQuestionResultAnswer, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}