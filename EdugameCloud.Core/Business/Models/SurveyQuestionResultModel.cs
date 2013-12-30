namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The SurveyQuestionResult model.
    /// </summary>
    public class SurveyQuestionResultModel : BaseModel<SurveyQuestionResult, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyQuestionResultModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SurveyQuestionResultModel(IRepository<SurveyQuestionResult, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}