using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class EventCollectionResult : CollectionResult<EventInfo>
    {
        public EventCollectionResult(StatusInfo status) : base(status)
        {
        }

        public EventCollectionResult(StatusInfo status, IEnumerable<EventInfo> values) : base(status, values)
        {
        }
    }
}
