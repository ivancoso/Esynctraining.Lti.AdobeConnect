using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect
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

        [DataMember(Name = "customization")]
        public CustomizationDTO Customization { get; set; }

    }

}
