using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class LearningPathCollectionResult : GenericCollectionResultBase<LearningPathItem>
    {
        public LearningPathCollectionResult(StatusInfo status) : base(status)
        {
        }

        public LearningPathCollectionResult(StatusInfo status, IEnumerable<LearningPathItem> values)
            : base(status, values)
        {
        }

        public LearningPathCollectionResult(StatusInfo status, IEnumerable<LearningPathItem> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        public string ScoId { get; set; }
    }
}
