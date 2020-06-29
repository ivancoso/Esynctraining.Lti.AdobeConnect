namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The question mapping
    /// </summary>
    public class QuestionForWeightBucketMap : BaseClassMap<QuestionForWeightBucket>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionForWeightBucketMap"/> class. 
        /// </summary>
        public QuestionForWeightBucketMap()
        {
            this.Map(x => x.TotalWeightBucket).Nullable();
            this.Map(x => x.WeightBucketType).Nullable();
            this.Map(x => x.PageNumber).Nullable();
            this.Map(x => x.AllowOther).Nullable();
            this.Map(x => x.IsMandatory).Not.Nullable().Default("1");

            this.References(x => x.Question);
        }

        #endregion
    }
}