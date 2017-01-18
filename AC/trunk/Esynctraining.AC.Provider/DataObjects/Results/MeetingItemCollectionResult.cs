namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    public class MeetingItemCollectionResult : CollectionResult<MeetingItem>
    {
        public MeetingItemCollectionResult(StatusInfo status)
            : base(status)
        {
        }
        
        public MeetingItemCollectionResult(StatusInfo status, IEnumerable<MeetingItem> values)
            : base(status, values)
        {
        }

    }

}