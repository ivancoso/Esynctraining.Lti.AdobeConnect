namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The test question result mapping
    /// </summary>
    public class TestQuestionResultMap : BaseClassMap<TestQuestionResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestQuestionResultMap"/> class. 
        /// </summary>
        public TestQuestionResultMap()
        {
            this.Map(x => x.Question).Length(500).Not.Nullable();
            this.Map(x => x.IsCorrect).Not.Nullable();
            this.References(x => x.TestResult).Not.Nullable();
            this.References(x => x.QuestionRef).Not.Nullable();
            this.References(x => x.QuestionType).Not.Nullable();
        }

        #endregion
    }
}