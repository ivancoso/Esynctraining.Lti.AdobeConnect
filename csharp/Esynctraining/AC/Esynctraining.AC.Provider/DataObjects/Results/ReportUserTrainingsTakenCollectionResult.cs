using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class ReportUserTrainingsTakenCollectionResult : CollectionResult<UserTrainingsTaken>
    {
        public ReportUserTrainingsTakenCollectionResult(StatusInfo status) : base(status)
        {
        }

        public ReportUserTrainingsTakenCollectionResult(StatusInfo status, IEnumerable<UserTrainingsTaken> values)
            : base(status, values)
        {
        }

        public ReportUserTrainingsTakenCollectionResult(StatusInfo status, IEnumerable<UserTrainingsTaken> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        public string ScoId { get; set; }
    }
}