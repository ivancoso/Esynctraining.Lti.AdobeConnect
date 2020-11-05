namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using System.Collections.Generic;

    using Esynctraining.AC.Provider.Entities;

    public class EventParticipantsCompleteInformationCollectionResult : CollectionResult<EventParticipantCompleteInformation>
    {
        public EventParticipantsCompleteInformationCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        public EventParticipantsCompleteInformationCollectionResult(StatusInfo status, IEnumerable<EventParticipantCompleteInformation> values)
            : base(status, values)
        {
        }

    }
}