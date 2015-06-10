using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class GeneratedRecordingJobCollectionResult : GenericCollectionResultBase<GeneratedRecordingJob>
    {
        public GeneratedRecordingJobCollectionResult(StatusInfo status)
            : base(status)
        {

        }

        public GeneratedRecordingJobCollectionResult(StatusInfo ststus, IEnumerable<GeneratedRecordingJob> generatedRecordings)
            : base(ststus, generatedRecordings)
        {

        }
    }
}
