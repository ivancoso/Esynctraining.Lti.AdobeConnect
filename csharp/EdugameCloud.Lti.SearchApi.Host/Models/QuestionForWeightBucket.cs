using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuestionForWeightBucket
    {
        public int QuestionForWeightBucketId { get; set; }
        public int QuestionId { get; set; }
        public decimal? TotalWeightBucket { get; set; }
        public int? WeightBucketType { get; set; }
        public bool? AllowOther { get; set; }
        public int? PageNumber { get; set; }
        public bool IsMandatory { get; set; }

        public virtual Question Question { get; set; }
    }
}
