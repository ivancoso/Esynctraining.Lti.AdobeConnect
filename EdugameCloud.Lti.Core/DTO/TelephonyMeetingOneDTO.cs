using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class TelephonyMeetingOneDTO
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string SecretHashKey { get; set; }

        [DataMember]
        public string OwningAccountNumber { get; set; }

    }

}
