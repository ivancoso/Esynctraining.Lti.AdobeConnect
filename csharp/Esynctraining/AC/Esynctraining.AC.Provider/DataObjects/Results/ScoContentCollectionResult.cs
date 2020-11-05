using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class ScoContentCollectionResult : CollectionResult<ScoContent>
    {
        public string ScoId { get; private set; }


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
        
    }

}
