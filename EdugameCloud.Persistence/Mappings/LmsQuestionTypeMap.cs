namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    /// <summary>
    /// The LMS question type map.
    /// </summary>
    public class LmsQuestionTypeMap : BaseClassMap<LmsQuestionType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsQuestionTypeMap"/> class.
        /// </summary>
        public LmsQuestionTypeMap()
        {
            this.Map(x => x.LmsQuestionTypeName).Column("lmsQuestionType").Not.Nullable();
            this.Map(x => x.QuestionTypeId).Not.Nullable();
            this.References(x => x.LmsProvider).Not.Nullable();
        }

        #endregion
    }
}
