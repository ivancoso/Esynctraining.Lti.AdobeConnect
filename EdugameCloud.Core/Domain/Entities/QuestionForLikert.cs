namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForLikert : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForLikert"/> class.
        /// </summary>
        public QuestionForLikert()
            : base(QuestionTypeEnum.RateScaleLikert)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the allow other.
        /// </summary>
        public virtual bool? AllowOther { get; set; }

        #endregion
    }
}