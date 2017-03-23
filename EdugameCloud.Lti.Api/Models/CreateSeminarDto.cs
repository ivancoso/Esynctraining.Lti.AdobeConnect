using EdugameCloud.Lti.DTO;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class CreateSeminarDto : MeetingDTOInput
    {
        [DataMember]
        public string seminarLicenseId { get; set; }
    }
}
