using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class RecordingsDto
    {
        public IEnumerable<ZoomRecordingSessionDto> ZoomRecordings { get; set; }
        public IEnumerable<ExternalRecordingsDto> ExternalRecordings { get; set; }
    }
}