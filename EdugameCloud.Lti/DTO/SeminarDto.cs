using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarDto : MeetingDTO
    {
        [DataMember(Name = "sessions")]
        public SeminarSessionDto[] Sessions { get; set; }

        [DataMember(Name = "seminarLicenseId")]
        public string SeminarLicenseId { get; set; }

    }

}
