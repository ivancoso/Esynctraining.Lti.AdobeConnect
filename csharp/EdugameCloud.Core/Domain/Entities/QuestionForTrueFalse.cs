namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForTrueFalse : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForTrueFalse"/> class. 
        /// </summary>
        public QuestionForTrueFalse() : base(QuestionTypeEnum.TrueFalse)
        {
        }

        #endregion
    }
}