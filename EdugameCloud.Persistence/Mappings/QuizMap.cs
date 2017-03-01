namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The quiz mapping
    /// </summary>
    public class QuizMap : BaseClassMap<Quiz>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizMap"/> class. 
        /// </summary>
        public QuizMap()
        {
            this.Map(x => x.QuizName).Length(100).Not.Nullable();
            this.Map(x => x.Description).Nullable();
            this.Map(x => x.IsPostQuiz);
            this.Map(x => x.PassingScore);
            this.Map(x => x.Guid);

            this.HasMany(x => x.Results).ExtraLazyLoad().Cascade.Delete().Inverse();

            this.References(x => x.SubModuleItem).Nullable();
            this.References(x => x.QuizFormat).Nullable();
            this.References(x => x.ScoreType).Nullable();

            this.Map(x => x.LmsQuizId).Nullable();
        }

        #endregion
    }
}