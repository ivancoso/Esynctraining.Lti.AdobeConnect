namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForSingleMultipleChoice : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForSingleMultipleChoice"/> class. 
        /// </summary>
        public QuestionForSingleMultipleChoice() : base(QuestionTypeEnum.SingleMultipleChoiceImage, QuestionTypeEnum.SingleMultipleChoiceText)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the allow other.
        /// </summary>
        public virtual bool? AllowOther { get; set; }

        /// <summary>
        /// Gets or sets the restrictions.
        /// </summary>
        public virtual string Restrictions { get; set; }

        #endregion
    }
}