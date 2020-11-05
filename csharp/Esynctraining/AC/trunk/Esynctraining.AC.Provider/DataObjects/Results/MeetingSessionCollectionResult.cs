namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    public class MeetingSessionCollectionResult : CollectionResult<MeetingSession>
    {
        public MeetingSessionCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        public MeetingSessionCollectionResult(StatusInfo status, IEnumerable<MeetingSession> values)
            : base(status, values)
        {
        }

    }
}