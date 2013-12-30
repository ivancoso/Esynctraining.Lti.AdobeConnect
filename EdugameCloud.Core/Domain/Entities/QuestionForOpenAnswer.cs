namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForOpenAnswer : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForOpenAnswer"/> class. 
        /// </summary>
        public QuestionForOpenAnswer() : base(QuestionTypeEnum.OpenAnswerMultiLine, QuestionTypeEnum.OpenAnswerSingleLine)
        {
        }

        #endregion
        #region Public Properties

        /// <summary>
        ///     Gets or sets the restrictions.
        /// </summary>
        public virtual string Restrictions { get; set; }

        #endregion
    }
}