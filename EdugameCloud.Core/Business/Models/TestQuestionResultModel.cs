namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The TestQuestionResult model.
    /// </summary>
    public class TestQuestionResultModel : BaseModel<TestQuestionResult, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestQuestionResultModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TestQuestionResultModel(IRepository<TestQuestionResult, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}