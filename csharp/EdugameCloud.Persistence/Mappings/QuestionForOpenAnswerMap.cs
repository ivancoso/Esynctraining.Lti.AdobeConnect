namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The question mapping
    /// </summary>
    public class QuestionForOpenAnswerRateMap : BaseClassMap<QuestionForOpenAnswer>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForOpenAnswerRateMap"/> class. 
        /// </summary>
        public QuestionForOpenAnswerRateMap()
        {
            this.Map(x => x.Restrictions).Nullable();
            this.Map(x => x.PageNumber).Nullable();
            this.Map(x => x.IsMandatory).Not.Nullable().Default("1");

            this.References(x => x.Question);
        }

        #endregion
    }
}