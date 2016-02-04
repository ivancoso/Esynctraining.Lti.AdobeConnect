using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class ACDetailsDTO
    {
        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "timeZoneShiftMinutes")]
        public int TimeZoneShiftMinutes { get; set; }

        [DataMember(Name = "passwordPolicies")]
        public ACPasswordPoliciesDTO PasswordPolicies { get; set; }

    }

}
