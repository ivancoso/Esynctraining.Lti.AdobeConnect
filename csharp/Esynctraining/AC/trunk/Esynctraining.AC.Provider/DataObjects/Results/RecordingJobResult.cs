using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class RecordingJobResult : ResultBase
    {
        public RecordingJobResult(StatusInfo status)
            : base(status)
        {

        }

        public RecordingJobResult(StatusInfo status, RecordingJob recordingJob)
            : base(status)
        {
            this.RecordingJob = recordingJob;
        }

        public RecordingJob RecordingJob { get; private set; }
    }
}
