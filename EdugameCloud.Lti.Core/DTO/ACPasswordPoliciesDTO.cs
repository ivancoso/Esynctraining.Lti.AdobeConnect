using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    public class ACPasswordPoliciesDTO
    {
        [DataMember]
        public bool passwordRequiresDigit { get; set; }

        [DataMember]
        public bool passwordRequiresCapitalLetter { get; set; }

        [DataMember]
        public string passwordRequiresSpecialChars { get; set; }

        [DataMember]
        public int passwordMinLength { get; set; }

        [DataMember]
        public int passwordMaxLength { get; set; }

    }

}
