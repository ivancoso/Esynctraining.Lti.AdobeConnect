using System.Collections.Generic;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class ExternalRecordingsDto
    {
        public ExternalStorageProvider ProviderId { get; set; }
        public IEnumerable<ExternalMediaDto> Recordings { get; set; }
    }
}