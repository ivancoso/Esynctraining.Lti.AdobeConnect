using System.Collections.Generic;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class RecordingsDto
    {
        public IEnumerable<ZoomRecordingSessionDto> ZoomRecordings { get; set; }
        public IEnumerable<ExternalRecordingsDto> ExternalRecordings { get; set; }
        public KalturaSessionDto KalturaDto { get; set; } //Kaltura session
    }
}