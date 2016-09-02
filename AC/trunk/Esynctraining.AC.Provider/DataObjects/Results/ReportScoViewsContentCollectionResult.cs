using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class ReportScoViewsContentCollectionResult : CollectionResult<ReportScoViewContent>
    {
        public ReportScoViewsContentCollectionResult(StatusInfo status) : base(status)
        {
        }

        public ReportScoViewsContentCollectionResult(StatusInfo status, IEnumerable<ReportScoViewContent> values)
            : base(status, values)
        {
        }

        public ReportScoViewsContentCollectionResult(StatusInfo status, IEnumerable<ReportScoViewContent> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        public string ScoId { get; set; }
    }
}