using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class AddExternalRecordingDto
    {
        public int ProviderId { get; set; }
        public string ProviderFileRecordId { get; set; }
    }
}