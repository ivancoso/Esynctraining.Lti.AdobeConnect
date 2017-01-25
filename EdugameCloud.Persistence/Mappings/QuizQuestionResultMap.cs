namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz question result mapping
    /// </summary>
    public class QuizQuestionResultMap : BaseClassMap<QuizQuestionResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionResultMap"/> class. 
        /// </summary>
        public QuizQuestionResultMap()
        {
            this.Map(x => x.Question).Length(500).Not.Nullable();
            this.Map(x => x.IsCorrect).Not.Nullable();
            this.References(x => x.QuizResult).Not.Nullable();
            this.References(x => x.QuestionRef).Not.Nullable();
            this.References(x => x.QuestionType).Not.Nullable();
            this.HasMany(x => x.Answers).Cascade.Delete().ExtraLazyLoad().Inverse();

        }

        #endregion
    }
}