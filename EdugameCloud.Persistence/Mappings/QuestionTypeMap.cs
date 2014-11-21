namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The EdugameQuestionType mapping
    /// </summary>
    public class QuestionTypeMap : BaseClassMap<QuestionType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionTypeMap"/> class. 
        /// </summary>
        public QuestionTypeMap()
        {
            this.Map(x => x.Type).Length(50).Nullable();
            this.Map(x => x.QuestionTypeOrder).Nullable();
            this.Map(x => x.QuestionTypeDescription).Length(200).Nullable();
            this.Map(x => x.Instruction).Length(500).Nullable();
            this.Map(x => x.CorrectText).Length(500).Nullable();
            this.Map(x => x.IncorrectMessage).Length(500).Nullable();
            this.Map(x => x.IconSource).Length(500).Nullable();
            this.Map(x => x.IsActive).Nullable();
        }

        #endregion
    }
}