using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class ScoContentCollectionResult : GenericCollectionResultBase<ScoContent>
    {
        public ScoContentCollectionResult(StatusInfo status) : base(status)
        {
        }

        public ScoContentCollectionResult(StatusInfo status, IEnumerable<ScoContent> values)
            : base(status, values)
        {
        }

        public ScoContentCollectionResult(StatusInfo status, IEnumerable<ScoContent> values, string scoId)
            : base(status, values)
        {
            this.ScoId = scoId;
        }

        public string ScoId { get; set; }
    }
}
