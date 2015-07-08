using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class RecordingJobCollectionResult : GenericCollectionResultBase<RecordingJob>
    {
        public RecordingJobCollectionResult(StatusInfo status)
            : base(status)
        {
        }

        public RecordingJobCollectionResult(StatusInfo status, IEnumerable<RecordingJob> values)
            : base(status, values)
        {
        }

    }
}
