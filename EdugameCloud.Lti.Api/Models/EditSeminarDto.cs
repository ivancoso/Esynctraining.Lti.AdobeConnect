using EdugameCloud.Lti.DTO;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class EditSeminarDto : MeetingDTOInput
    {
        [DataMember]
        public string seminarLicenseId { get; set; }

        [DataMember]
        public string lmsProviderName { get; set; }
    }
}
