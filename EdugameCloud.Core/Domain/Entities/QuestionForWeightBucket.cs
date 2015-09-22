namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    ///     The question.
    /// </summary>
    public class QuestionForWeightBucket : QuestionFor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForWeightBucket"/> class. 
        /// </summary>
        public QuestionForWeightBucket() : base(QuestionTypeEnum.WeightedBucketRatio)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        public virtual decimal? TotalWeightBucket { get; set; }

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        public virtual int? WeightBucketType { get; set; }

        /// <summary>
        /// Gets or sets the allow other.
        /// </summary>
        public virtual bool? AllowOther { get; set; }

        #endregion
    }
}