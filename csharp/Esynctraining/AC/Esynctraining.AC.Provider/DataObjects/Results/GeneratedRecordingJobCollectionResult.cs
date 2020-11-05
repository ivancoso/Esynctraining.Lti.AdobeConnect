using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class GeneratedRecordingJobCollectionResult : CollectionResult<GeneratedRecordingJob>
    {
        public GeneratedRecordingJobCollectionResult(StatusInfo status)
            : base(status)
        {

        }

        public GeneratedRecordingJobCollectionResult(StatusInfo status, IEnumerable<GeneratedRecordingJob> generatedRecordings)
            : base(status, generatedRecordings)
        {

        }
    }
}
