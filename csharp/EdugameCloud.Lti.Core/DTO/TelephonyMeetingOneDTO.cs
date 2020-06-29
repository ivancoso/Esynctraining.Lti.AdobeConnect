using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class TelephonyMeetingOneDTO
    {
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "secretHashKey")]
        public string SecretHashKey { get; set; }

        [DataMember(Name = "owningAccountNumber")]
        public string OwningAccountNumber { get; set; }

    }

}
