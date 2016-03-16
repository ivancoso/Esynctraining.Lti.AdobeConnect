using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class SeminarLicenseDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "rooms")]
        public SeminarDto[] Rooms { get; set; }

    }

}
