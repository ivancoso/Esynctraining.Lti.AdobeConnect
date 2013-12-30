namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForRate : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForRate"/> class. 
        /// </summary>
        public QuestionForRate() : base(QuestionTypeEnum.Rate)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the restrictions.
        /// </summary>
        public virtual string Restrictions { get; set; }

        /// <summary>
        ///     Gets or sets the allow other.
        /// </summary>
        public virtual bool? AllowOther { get; set; }

        #endregion
    }
}